using System;
using Bopple.Core.Models;

namespace Bopple.Core.EventHandling
{
    public static class GameEvents
    {
        public class InRound
        {
            #region All Bopples Dead

            public static event Action OnAllBopplesDead;

            public static void TriggerAllBopplesDead()
            {
                OnAllBopplesDead?.Invoke();
            }

            #endregion

            #region Launch

            public static event Action OnLaunch;

            public static void TriggerLaunch()
            {
                OnLaunch?.Invoke();
            }

            #endregion
        }

        public class GameCycle
        {
            #region Game Start

            public static event Action<GameOptions> OnGameStart;

            public static void TriggerGameStart(GameOptions options)
            {
                OnGameStart?.Invoke(options);
            }

            public static void TriggerGameStart() => TriggerGameStart(GameOptions.Default);

            #endregion

            #region Back To Main Menu

            public static event Action<bool> OnBackToMainMenu;

            public static void TriggerBackToMainMenu(bool save = true)
            {
                OnBackToMainMenu?.Invoke(save);
            }

            #endregion
        }

        public class AppState
        {
            #region Quit

            public static event Action<bool, bool> OnQuit;

            public static void TriggerQuit(bool save = true, bool crash = false)
            {
                OnQuit?.Invoke(save, crash);
            }

            #endregion
        }
    }
}