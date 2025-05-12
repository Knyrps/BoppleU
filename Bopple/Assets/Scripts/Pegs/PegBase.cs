using UnityEngine;

namespace Scripts.Pegs
{
    public abstract class PegBase : MonoBehaviour
    {
        public float addBounceForcePercent = 0.6f;
        public int hitsNeeded = 3;
        public bool finalBounce = true;

        protected int TimesHit = 0;

        public abstract string PegName { get; }

        public abstract string PegShortInfo { get; }

        public abstract Sprite PegSprite { get; }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObjects.Bopple bopple = collision.gameObject.GetComponent<GameObjects.Bopple>();
            if (!bopple)
            {
                return;
            }

            this.TimesHit++;

            HandleCollision(collision);

            if (!this.finalBounce && this.TimesHit >= this.hitsNeeded)
            {
                HandleDeath();
                return;
            }

            Rigidbody2D rb = collision.collider.attachedRigidbody;
            if (rb)
            {
                Vector2 bounceDirection = (collision.transform.position - this.transform.position).normalized;
                float bounceForce = rb.linearVelocity.magnitude * this.addBounceForcePercent;
                Ricochet(bounceDirection, bounceForce, rb);
            }

            if (this.TimesHit >= this.hitsNeeded)
            {
                HandleDeath();
            }
        }

        protected virtual void HandleCollision(Collision2D originalCollision) { }

        protected virtual void HandleDeath()
        {
            Destroy(this.gameObject);
            Destroy(this);
        }

        protected virtual void Ricochet(Vector2 bounceDirection, float bounceForce, Rigidbody2D rb)
        {
            rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }
    }
}
