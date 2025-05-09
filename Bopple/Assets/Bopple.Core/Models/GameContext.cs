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
            this.Initialized = true;
        }

        private GameContext()
        { }

        public static GameContext Empty => new GameContext();
    }
}