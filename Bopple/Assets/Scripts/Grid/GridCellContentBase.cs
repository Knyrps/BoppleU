using System.Collections.Generic;
using Scripts.Grid.GridCellContentValidators;

namespace Scripts.Grid
{
    public abstract class GridCellContentBase
    {
        public List<GridCellContentValidatorBase> Validators { get; } = new();

        private GridCellMemberBase InnerContent;

        private GridCellEffectBase Effect;

        public void AddValidator(GridCellContentValidatorBase validator)
        {
            this.Validators.Add(validator);
        }

        public void RemoveValidator(GridCellContentValidatorBase validator)
        {
            this.Validators.Remove(validator);
        }

        public void SetInnerContent(GridCellMemberBase innerContent)
        {
            this.InnerContent = innerContent;
        }

        public void SetEffect(GridCellEffectBase effect)
        {
            this.Effect = effect;
        }

        // Default constructor
        protected GridCellContentBase()
        {
        }
    }

    public class StandardGridCellContent : GridCellContentBase
    {
    }
}