using UnityEngine;

namespace Scripts.GameObjects
{
    public class Peg : MonoBehaviour
    {
        public float AddBounceForcePercent = 0.6f;
        public int HitsNeeded = 3;
        public bool FinalBounce = true;

        private int timesHit = 0;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Bopple bopple = collision.gameObject.GetComponent<Bopple>();
            if (bopple == null)
            {
                return;
            }

            this.timesHit++;

            if (!this.FinalBounce && this.timesHit >= this.HitsNeeded)
            {
                Destroy(this.gameObject);
                return;
            }

            Rigidbody2D rb = collision.collider.attachedRigidbody;
            if (rb != null)
            {
                Vector2 bounceDirection = (collision.transform.position - this.transform.position).normalized;
                float bounceForce = rb.linearVelocity.magnitude * this.AddBounceForcePercent;
                rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
            }

            if (this.timesHit >= this.HitsNeeded)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
