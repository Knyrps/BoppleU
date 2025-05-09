using Bopple.Core.EventHandling;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Tooltip("The Main Game Scene")]
    public string MainGameSceneName;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void StartGame()
    {
        SceneManager.LoadScene(this.MainGameSceneName);
        GameEvents.GameCycle.TriggerGameStart();
    }

    private void QuitGame()
    {
        GameEvents.GameCycle.TriggerQuit();
    }
}
