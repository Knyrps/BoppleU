using UnityEngine;

namespace Scripts.Grid.GridCellContentValidators
{
    public abstract class GridCellContentValidatorBase : ScriptableObject
    {
        public abstract bool Validate(GridCell cell, GridCellContentBase content);
    }

    [CreateAssetMenu(fileName = "AlwaysFalse", menuName = "Grid/Cell Content Validators/Always False")]
    public class GridCellContentValidatorAlwaysFalse : GridCellContentValidatorBase
    {
        public override bool Validate(GridCell cell, GridCellContentBase content)
        {
            return false;
        }
    }
}