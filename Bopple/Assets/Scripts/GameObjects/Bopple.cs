using System.Linq;
using System.Xml.Linq;
using Bopple.Core.EventHandling;
using Bopple.Core.Utilities;
using UnityEngine;

namespace Scripts.GameObjects
{
    public class Bopple : MonoBehaviour
    {
        public Rigidbody2D RigidBody;

        private bool isMoving = false;
        private int instanceId;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            this.instanceId = this.GetInstanceID();
            this.RigidBody.bodyType = RigidbodyType2D.Static;
        }

        // Update is called once per frame
        void Update()
        {
            float maxSpeed = 50f;
            if (RigidBody.linearVelocity.magnitude > maxSpeed)
            {
                RigidBody.linearVelocity = RigidBody.linearVelocity.normalized * maxSpeed;
            }
        }

        void OnDestroy()
        {
            BoppleLauncher.ActiveBopples.Remove(this);
            Debug.Log($"Bopple #{this.instanceId} had a Lifetime of {StopwatchUtil.Stop("_Launch#" + this.instanceId)}ms.");

            if (!BoppleLauncher.ActiveBopples.Any())
            {
                GameEvents.InRound.TriggerAllBopplesDead();
            }
        }

        public bool Launch(float angle, float force, bool allowUpward = false)
        {
            if (this.isMoving)
            {
                return false;
            }

            Vector2 direction = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            if (direction.y > 0 && !allowUpward)
            {
                return false;
            }
            if (BoppleLauncher.ActiveBopples.Contains(this))
            {
                return false;
            }

            BoppleLauncher.ActiveBopples.Add(this);

            this.isMoving = true;

            this.RigidBody.bodyType = RigidbodyType2D.Dynamic;
            this.RigidBody.WakeUp(); // Ensure the Rigidbody is awake and responsive
            this.RigidBody.linearVelocity = Vector2.zero;
            this.RigidBody.angularVelocity = 0f; // Also explicitly zero out angular velocity
            this.RigidBody.totalForce = Vector2.zero; // Clearing totalForce is generally good practice too
            this.RigidBody.AddForce(direction * force, ForceMode2D.Impulse);

            GameEvents.InRound.TriggerLaunch();

            Debug.Log($"Bopple #{this.instanceId} launched with force: {force} in direction: {direction}. At time of launch, {BoppleLauncher.ActiveBopples.Count} Bopples were actively on screen.");

            StopwatchUtil.StartNew("_Launch#" + this.instanceId);

            return true;
        }
    }
}