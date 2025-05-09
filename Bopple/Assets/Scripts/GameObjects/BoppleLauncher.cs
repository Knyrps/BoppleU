using System.Collections;
using System.Collections.Generic;
using Bopple.Core.EventHandling;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.GameObjects
{
    public class BoppleLauncher : MonoBehaviour
    {
        [Tooltip("The Bopple prefab.")]
        public GameObject BopplePrefab;

        [Tooltip("The force to apply to Bopple on launch.")]
        public float LaunchVelocity = 7f;

        [Tooltip("Whether to allow launching Bopple vertically upwards.")]
        public bool AllowUpwardLaunch = false;

        [Tooltip("This is a fun setting. Whether to allow infinite Bopple stock. If true, the launcher will restock instantly after launching a Bopple.")]
        public bool Infinite = false;

        [Tooltip("The duration of the spawn animation for the Bopple.")]
        public float SpawnAnimationDuration = 0.25f;

        private Bopple stock = null;
        private bool stocked = false;
        private bool aiming = false;

        public static List<Bopple> ActiveBopples = new();

        void Start()
        {
            Restock();

            GameEvents.InRound.OnAllBopplesDead += Restock;
        }

        void Update()
        {
            if (this.stocked && this.stock != null)
            {
                if (!this.aiming && Mouse.current.leftButton.isPressed)
                {
                    this.aiming = true;
                }
                else if (this.aiming && !Mouse.current.leftButton.isPressed)
                {
                    if (TryGetDirectionFromCursor(out float direction))
                    {
                        Debug.Log("Launch direction: " + direction + "deg");
                        if (this.stock.Launch(direction, this.LaunchVelocity, this.AllowUpwardLaunch))
                        {
                            this.stock = null;
                            this.stocked = false;

                            if (this.Infinite)
                            {
                                Restock();
                            }
                        }
                        else
                        {
                            Debug.Log("Failed to launch Bopple");
                        }
                    }
                    this.aiming = false;
                }

                if (this.aiming)
                {
                    if (this.stock == null || !this.stocked)
                    {
                        return;
                    }

                    AimLine aimLine = this.gameObject.GetComponentInChildren<AimLine>();
                    if (aimLine != null)
                    {
                        aimLine.Render(this.transform.position, this.LaunchVelocity, this.stock.RigidBody.gravityScale, this.stock.RigidBody.linearDamping);
                    }
                }
            }
        }

        public void Restock()
        {
            if (this.stocked || this.stock != null)
            {
                Debug.Log("Already stocked");
                return;
            }

            this.stock = Instantiate(this.BopplePrefab).GetComponent<Bopple>();

            this.stock.gameObject.transform.position = this.transform.position;

            Vector2 initialScale = this.stock.transform.localScale;
            Quaternion initialRotation = this.stock.transform.rotation;

            this.stock.transform.localScale = Vector2.zero;
            this.stock.transform.rotation = Quaternion.Euler(Vector3.left);

            // The stock is spawned with an initial size of 0
            // Now, we animate it to its final size over 0.5 seconds
            StartCoroutine(ChangeStockScaleOverTime(initialScale, initialRotation, this.SpawnAnimationDuration));
        }

        private IEnumerator ChangeStockScaleOverTime(Vector2 endScale, Quaternion endRotation, float time)
        {
            Vector2 startScale = this.stock.transform.localScale;
            Quaternion startRotation = this.stock.transform.rotation;

            float elapsedTime = 0f;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / time);

                this.stock.transform.localScale = Vector2.Lerp(startScale, endScale, progress);
                this.stock.gameObject.GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Lerp(startRotation, endRotation, progress);

                yield return null;
            }

            this.stock.transform.localScale = endScale;
            this.stock.gameObject.GetComponent<SpriteRenderer>().transform.rotation = endRotation;

            this.stocked = true;
        }

        private bool TryGetDirectionFromCursor(out float dir)
        {
            dir = 0f;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = mousePos - new Vector2(this.transform.position.x, this.transform.position.y);

            if (direction.y > 0 && !this.AllowUpwardLaunch)
            {
                return false;
            }

            dir = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            return true;
        }
    }
}
