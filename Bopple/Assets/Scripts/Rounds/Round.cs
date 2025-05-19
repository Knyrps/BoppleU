using System;

namespace Scripts.Rounds
{
    public class Round
    {
        public int RoundNumber { get; set; }

        public RoundType RoundType { get; set; }

        public bool IsSpecialRound => this.RoundType != RoundType.Normal;

        public bool IsNormalRound => this.RoundType == RoundType.Normal;

        public void Step()
        {
            this.RoundNumber++;
            this.OnStartRound(this.RoundNumber);
        }

        public event Action<int> StartRound;

        private void OnStartRound(int number)
        {
            this.StartRound?.Invoke(number);
        }
    }
}