using System.Collections.Generic;

namespace Scripts.Grid
{
    public abstract class GridCellContentValidator
    {
        public virtual bool Validate(GridCell cell, GridCellContentBase content)
        {
            return true;
        }

        public static List<GridCellContentValidator> AlwaysThrowFalse { get; } = new()
        {
            new GridCellContentValidatorAlwaysFalse()
        };
    }

    public class GridCellContentValidatorAlwaysFalse : GridCellContentValidator
    {
        public override bool Validate(GridCell cell, GridCellContentBase content)
        {
            return false;
        }
    }
}