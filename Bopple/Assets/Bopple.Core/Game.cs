using Bopple.Core.Enums;
using Bopple.Core.Models;

namespace Bopple.Core
{
    public static class Game
    {
        public static GameState State { get; private set; }

        public static GameOptions Options { get; private set; }

        public static GameContext Context { get; private set; }

        /// <summary>
        /// On App Start.
        /// </summary>
        public static void Initialize()
        {
            State = GameState.None;
            Options = GameOptions.Empty;
            Context = GameContext.Empty;
        }
    }
}