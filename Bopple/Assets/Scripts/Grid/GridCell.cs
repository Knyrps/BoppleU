using System;
using Scripts.Pegs;
using UnityEngine;

namespace Scripts.Grid
{
    public class GridCell : MonoBehaviour
    {
        #region Unity Serialized

        [SerializeField]
        private (bool State, int? RoundsLeft) isDisabled;

        [SerializeField]
        private Sprite cellActiveSprite;

        [SerializeField]
        private Sprite cellDisabledSprite;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        #endregion

        #region Private fields and properties

        private PegBase _peg;

        private PegBase Peg
        {
            get => this._peg;
            set
            {
                this._peg = value;
                this._peg.transform.position = this.transform.position;
            }
        }

        private GridCellEffectBase Effect { get; set; } = null!;

        #endregion

        #region Constructors and static intitializers

        private void Awake()
        {
            this.Enable();
            this.SetSprite(this.cellActiveSprite);
        }

        #endregion

        #region Public methods

        public void Disable(int? rounds = null)
        {
            this.isDisabled = (true, rounds);
            this.SetSprite(this.cellDisabledSprite);
        }

        public void Enable()
        {
            this.isDisabled = (false, null);
            this.SetSprite(this.cellActiveSprite);
        }

        public (bool, int?) IsDisabled() => this.isDisabled;

        public void UpdatePositionAndSize(Vector2 position, float size)
        {
            // Position the object (in world space)
            this.gameObject.transform.position = new Vector3(position.x, position.y, this.transform.position.z);

            // Uniformly scale the object (assuming it's square)
            this.gameObject.transform.localScale = new Vector3(size, size, this.transform.localScale.z);
        }

        public void ApplyEffect<T>(T effect) where T : GridCellEffectBase
        {
            if (!effect)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            this.SetEffect(effect);
        }

        public void PopulateWithPeg<T>(T peg) where T : PegBase
        {
            if (!peg)
            {
                throw new ArgumentNullException(nameof(peg));
            }
            this.SetPeg(peg);
        }

        #endregion

        #region Private Methods

        private void SetSprite(Sprite sprite)
        {
            this.spriteRenderer.sprite = sprite;
            this.spriteRenderer.size = Vector2.one;
        }

        private bool ValidateContent(PegBase peg = null, GridCellEffectBase effect = null)
        {
            if (!peg)
            {
                peg = this.Peg;
            }

            if (!effect)
            {
                effect = this.Effect;
            }

            // Cell is empty
            if (!peg && !effect)
            {
                return true;
            }

            // TODO: Check if the current effect is valid for the current peg or vice versa

            return true;
        }

        private bool SetPeg(PegBase peg)
        {
            if (!this.ValidateContent(peg: peg))
            {
                return false;
            }

            this.Peg = peg;
            return true;
        }

        private bool SetEffect(GridCellEffectBase effect)
        {
            if (!this.ValidateContent(effect: effect))
            {
                return false;
            }

            this.Effect = effect;
            return true;
        }

        #endregion
    }
}