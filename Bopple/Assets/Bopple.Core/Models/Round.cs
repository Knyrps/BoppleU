using Bopple.Core.Enums;

namespace Bopple.Core.Models
{
    public class Round
    {
        public int RoundNumber { get; set; }
        
        public RoundType RoundType { get; set; }

        public bool IsSpecialRound => this.RoundType != RoundType.Normal;

        public bool IsNormalRound => this.RoundType == RoundType.Normal;

        public void Step()
        {
            
        }
    }
}