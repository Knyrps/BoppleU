using UnityEngine;

namespace Scripts.GameObjects
{
    public class GameStateController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Playfield Controller")]
        private GameObject PlayFieldController;

        private PlayFieldController playFieldController;

        void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            this.playFieldController = this.PlayFieldController.GetComponent<PlayFieldController>();

            this.playFieldController.TryInitializeGrid();
        }
    }
}
