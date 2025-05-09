using System.Collections.Generic;
using Bopple.Core.EventHandling;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimLine : MonoBehaviour
{
    [Tooltip("Prefab for the dots along the trajectory.")]
    public GameObject DotPrefab;

    [Tooltip("Number of points to define the trajectory line and dots.")]
    [Min(2)] // Resolution should be at least 2 for meaningful output
    public int Resolution = 20;

    [Tooltip("Scale of the first dot along the trajectory. 0 = no scale, 1 = full scale.")]
    public float InitialDotScale = 1f;

    [Tooltip("Scale of the final dots along the trajectory. 0 = no scale, 1 = full scale.")]
    public float FinalDotScale = 0.7f;

    [Tooltip("Opacity of the final dots along the trajectory. 0 = fully transparent, 1 = fully opaque.")]
    public float FinalDotOpacity = 0.4f;

    [Tooltip("The length of the aim line.")]
    public float AimLineLength = 2f;

    [Tooltip("Aim line offset from the launcher.")]
    public float AimLineOffset = 0.5f;

    private List<GameObject> dots = new List<GameObject>();

    // Constants for the numerical integration in DetermineTimeForFlightPathLength
    // Time step for simulating path length calculation (smaller is more accurate but more costly)
    private const float PATH_LENGTH_CALCULATION_TIME_STEP = 0.01f;

    // Maximum simulation time to prevent infinite loops if desired length is unreachable
    private const float PATH_LENGTH_CALCULATION_MAX_SIM_TIME = 10f;

    void Start()
    {
        GameEvents.InRound.OnLaunch += ClearDots;
    }

    private Vector2 lastRender;

    private bool RequireReRender(Vector2 data)
    {
        bool result = this.lastRender.Equals(Vector2.negativeInfinity) || !this.lastRender.Equals(data);

        if (result)
        {
            this.lastRender = data;
        }

        return result;
    }

    public void Render(Vector2 origin, float force, float gravity, float linDamp)
    {
        // Get mouse position in world space
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldMouse = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
        Vector2 direction = (worldMouse - origin).normalized;

        if (RequireReRender(worldMouse))
        {
            RenderLineInternal(origin, direction, force, gravity);
            return;
        }
    }

    /// <summary>
    /// Calculates the position of a projectile at a given time using the defined physics model.
    /// </summary>
    /// <param name="origin">Starting point of the projectile.</param>
    /// <param name="initialVelocity">Initial velocity of the projectile.</param>
    /// <param name="effectiveGravityFactor">The gravity factor (original gravity * 5).</param>
    /// <param name="time">Time elapsed since launch.</param>
    /// <returns>The calculated position of the projectile at the given time.</returns>
    private static Vector2 CalculateProjectilePoint(Vector2 origin, Vector2 initialVelocity,
        float effectiveGravityFactor, float time)
    {
        // Original physics model: s = v0*t + 0.5*a*t^2, where gravity effect is quadratic.
        // effectiveGravityFactor is (gravity * 5) from the original code.
        Vector2 gravityDisplacement = new Vector2(0f, effectiveGravityFactor * time * time);
        return origin + (initialVelocity * time) - gravityDisplacement;
    }

    /// <summary>
    /// Determines the time it takes for the projectile to travel a specific distance along its flight path.
    /// This is done by numerically integrating the path length.
    /// </summary>
    /// <param name="desiredPathLength">The target length of the flight path.</param>
    /// <param name="origin">Starting point of the projectile.</param>
    /// <param name="initialVelocity">Initial velocity of the projectile.</param>
    /// <param name="effectiveGravityFactor">The gravity factor (original gravity * 5).</param>
    /// <returns>The time (in seconds) taken to reach the desired path length,
    /// or PATH_LENGTH_CALCULATION_MAX_SIM_TIME if the length is unreachable within that simulation time.</returns>
    private static float DetermineTimeForFlightPathLength(
        float desiredPathLength,
        Vector2 origin,
        Vector2 initialVelocity,
        float effectiveGravityFactor)
    {
        // If desired length is negligible, no time is needed.
        if (desiredPathLength <= 0.0001f)
        {
            return 0f;
        }

        float accumulatedPathLength = 0f;
        float timeAtPreviousPoint = 0f;
        Vector2 previousPoint = origin;

        // Calculate max iterations to prevent excessively long calculations.
        int maxSteps = Mathf.CeilToInt(PATH_LENGTH_CALCULATION_MAX_SIM_TIME / PATH_LENGTH_CALCULATION_TIME_STEP);

        for (int step = 0; step < maxSteps; ++step)
        {
            float timeAtCurrentPoint = timeAtPreviousPoint + PATH_LENGTH_CALCULATION_TIME_STEP;
            Vector2 currentPoint = CalculateProjectilePoint(origin, initialVelocity, effectiveGravityFactor,
                timeAtCurrentPoint);
            float segmentLength = Vector2.Distance(previousPoint, currentPoint);

            // Check for stagnation: if no movement, no initial velocity, and no gravity,
            // and desired length is positive, it's unreachable.
            if (segmentLength < 0.00001f && initialVelocity.sqrMagnitude < 0.00001f &&
                Mathf.Approximately(effectiveGravityFactor, 0f))
            {
                return desiredPathLength > 0.0001f ? PATH_LENGTH_CALCULATION_MAX_SIM_TIME : 0f;
            }

            // If adding the current segment exceeds or meets the desired path length
            if (accumulatedPathLength + segmentLength >= desiredPathLength)
            {
                float neededLengthInSegment = desiredPathLength - accumulatedPathLength;
                // Avoid division by zero if segmentLength is tiny (e.g., at apex of vertical shot)
                if (segmentLength < 0.00001f)
                {
                    return timeAtCurrentPoint; // Close enough
                }

                // Interpolate the time within the last segment to get a more precise value
                float proportionOfSegment = Mathf.Clamp01(neededLengthInSegment / segmentLength);
                return timeAtPreviousPoint + (PATH_LENGTH_CALCULATION_TIME_STEP * proportionOfSegment);
            }

            accumulatedPathLength += segmentLength;
            previousPoint = currentPoint;
            timeAtPreviousPoint = timeAtCurrentPoint;
        }

        // Fallback if no time was determined within the max simulation time
        return PATH_LENGTH_CALCULATION_MAX_SIM_TIME;
    }

    /// <summary>
    /// Style the dots along the trajectory line.
    /// This includes setting their opacity and scale based on their position along the trajectory.
    /// </summary>
    private void StyleDots()
    {
        if (this.DotPrefab == null || this.dots.Count <= 1)
        {
            return;
        }

        float opacityRange = 1 - this.FinalDotOpacity;
        float scaleRange = 1f - this.FinalDotScale;

        for (int idx = 0; idx < this.dots.Count; idx++)
        {
            GameObject dot = this.dots[idx];

            float progress = (float)idx / (this.dots.Count - 1);

            float opacity = this.FinalDotOpacity + (this.InitialDotScale - progress) * (this.InitialDotScale - this.FinalDotOpacity);
            float scale = this.FinalDotScale + (this.InitialDotScale - progress) * (this.InitialDotScale - this.FinalDotScale);

            SpriteRenderer dotRenderer = dot.GetComponent<SpriteRenderer>();

            if (dotRenderer == null)
            {
                continue;
            }

            dotRenderer.color = new Color(dotRenderer.color.r, dotRenderer.color.g, dotRenderer.color.b, opacity);
            dot.transform.localScale = new Vector3(scale, scale, dot.transform.localScale.z);
        }
    }

    /// <summary>
    /// Renders the trajectory line with a defined absolute path length.
    /// Points are spread evenly along this path.
    /// </summary>
    /// <param name="origin">Starting point of the trajectory.</param>
    /// <param name="direction">Initial direction of the trajectory (should be normalized).</param>
    /// <param name="force">Magnitude of the initial impulse/velocity.</param>
    /// <param name="gravity">Base gravity magnitude (will be scaled by 5 internally).</param>
    public void RenderLineInternal(Vector2 origin, Vector2 direction, float force, float gravity)
    {
        ClearDots();

        // Resolution must be at least 2.
        if (this.Resolution <= 1)
        {
            return; // Nothing to render
        }

        Vector2 initialVelocity = direction.normalized * force;
        Vector2[] points = new Vector2[this.Resolution];
        // The "magic number" 5 for gravity scaling is preserved from the original code.
        float effectiveGravityFactor = gravity * 5f;

        // Determine the total time it takes for the projectile to travel 'lineLength' along its path.
        float totalTimeForPath =
            DetermineTimeForFlightPathLength(this.AimLineLength, origin, initialVelocity, effectiveGravityFactor);

        for (int i = 0; i < this.Resolution; i++)
        {
            float t; // Time parameter for the current point

            t = ((totalTimeForPath / (float)(this.Resolution - 1)) * i);

            Vector2 point = CalculateProjectilePoint(origin, initialVelocity, effectiveGravityFactor, t);

            Vector2 directionFromOrigin = (point - origin).normalized;

            // Offset the point along the direction vector
            point += directionFromOrigin * this.AimLineOffset;

            points[i] = point;

            if (this.DotPrefab == null)
            {
                continue;
            }

            GameObject dot = Instantiate(this.DotPrefab, point, Quaternion.identity);
            this.dots.Add(dot);
        }

        StyleDots();
    }

    private void ClearDots()
    {
        this.lastRender = Vector2.negativeInfinity;

        foreach (GameObject dot in this.dots)
        {
            Destroy(dot);
        }

        this.dots.Clear();
    }
}
