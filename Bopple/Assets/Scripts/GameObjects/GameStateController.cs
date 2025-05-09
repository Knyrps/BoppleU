using UnityEngine;

public class GameStateController : MonoBehaviour
{
    [Tooltip("The Playfield Controller")]
    public GameObject PlayFieldController;

    private PlayFieldController playFieldController;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        this.playFieldController = this.PlayFieldController.GetComponent<PlayFieldController>();

        this.playFieldController.InitializeGrid();
    }
}
