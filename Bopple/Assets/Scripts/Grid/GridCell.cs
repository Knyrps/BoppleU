using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Grid
{
    public class GridCell : MonoBehaviour
    {
        public GridCellContentBase Content { get; private set; } = null!;

        [SerializeField]
        private bool isDisabled;

        [SerializeField]
        private Sprite cellActiveSprite;

        [SerializeField]
        private Sprite cellDisabledSprite;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        #region Constructors and static intitializers

        public GridCell(GridCellContentBase content = null, bool disabled = false)
        {
            this.isDisabled = disabled;
            this.Content = content;
        }

        private void Awake()
        {
            this.isDisabled = false;
            this.SetSprite(this.cellActiveSprite);
        }

        #endregion

        #region Public methods

        public void SetContent(GridCellContentBase content)
        {
            if (ValidateContent(this, content))
            {
                this.Content = content;
            }
        }

        public void Disable()
        {
            this.isDisabled = true;
            this.SetSprite(this.cellDisabledSprite);
        }

        public void Enable()
        {
            this.isDisabled = false;
            this.SetSprite(this.cellActiveSprite);
        }

        private void SetSprite(Sprite sprite)
        {
            this.spriteRenderer.sprite = sprite;
            this.spriteRenderer.size = Vector2.one;
        }

        public void UpdatePositionAndSize(Vector2 position, float size)
        {
            // Position the object (in world space)
            this.gameObject.transform.position = new Vector3(position.x, position.y, this.transform.position.z);

            // Uniformly scale the object (assuming it's square)
            this.gameObject.transform.localScale = new Vector3(size, size, this.transform.localScale.z);
        }


        private static bool ValidateContent(GridCell cell, GridCellContentBase content)
        {
            if (content == null)
            {
                return false;
            }

            foreach (GridCellContentValidator v in content.Validators)
            {
                if (!v.Validate(cell, content))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}