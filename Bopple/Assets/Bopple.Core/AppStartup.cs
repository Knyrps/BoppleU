using UnityEngine;

namespace Bopple.Core
{
    public class AppStartup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void SceneAwake()
        {
            Game.Initialize();
        }
    }
}