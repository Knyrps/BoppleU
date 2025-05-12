using UnityEngine;

namespace Scripts.Grid
{
    public abstract class GridCellEffectBase : ScriptableObject
    {
        public abstract string EffectName { get; }

        public abstract string EffectShortInfo { get; }

        public abstract Sprite EffectSprite { get; }
    }
}