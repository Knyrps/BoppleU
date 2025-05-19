using Bopple.Core.Extensions;
using UnityEngine;

namespace Scripts.Pegs
{
    [Weight(80)]
    public class StandardPeg : PegBase
    {
        public override string PegName => nameof(StandardPeg).PascalToReadable();

        public override string PegShortInfo { get; }

        public override Sprite PegSprite { get; }
    }
}