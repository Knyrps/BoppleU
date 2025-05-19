using Scripts.Rounds;

namespace Bopple.Core.Models
{
    public class GameContext
    {
        public Round CurrentRound { get; private set; }

        public bool Initialized { get; private set; }

        public void AdvanceRound()
        {
            this.CurrentRound.Step();
        }

        public void Initialize()
        {
            this.CurrentRound = new Round
            {
                RoundNumber = 0,
                RoundType = RoundType.Normal
            };
            this.Initialized = true;
        }

        private GameContext()
        { }

        public static GameContext Empty => new GameContext();
    }
}