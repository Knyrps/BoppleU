using Bopple.Core.Enums;

namespace Bopple.Core.Models
{
    public class GameOptions
    {
        public GameMode Mode { get; set; }

        public GameOptions()
        {
            this.Mode = GameMode.Normal;
        }

        public static GameOptions Default => new GameOptions
        {
            Mode = GameMode.Normal
        };

        public static GameOptions Empty => new GameOptions
        {
            Mode = GameMode.None
        };
    }
}