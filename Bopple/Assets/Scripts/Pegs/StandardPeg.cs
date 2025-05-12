using Bopple.Core.Extensions;
using UnityEngine;

namespace Scripts.Pegs
{
    public class StandardPeg : PegBase
    {
        public override string PegName => nameof(StandardPeg).PascalToReadable();

        public override string PegShortInfo { get; }

        public override Sprite PegSprite { get; }
    }
}