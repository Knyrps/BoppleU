using System.Collections.Generic;

namespace Scripts.Grid
{
    public abstract class GridCellContentBase
    {
        public List<GridCellContentValidator> Validators { get; } = new();

        public void AddValidator(GridCellContentValidator validator)
        {
            this.Validators.Add(validator);
        }

        public void RemoveValidator(GridCellContentValidator validator)
        {
            this.Validators.Remove(validator);
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