using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SG
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        private PlayerControls playerControls;
        public PlayerManager player;

        [Header("Inputs")]
        [SerializeField] private bool leftClick = false;

        [Header("Movement Position")]
        [SerializeField] Vector3 movementPosition;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // Событие статическое, поэтому отписываемся, иначе оно будет вызывать уничтоженный объект
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnSceneChanged(Scene currentScene, Scene nextScene)
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.buildIndex == WorldSaveGameManager.instance.menuSceneIndex)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
                playerControls = new PlayerControls();

            // Именованный метод можно отписать в OnDisable, лямбду i => ... нельзя
            playerControls.Player.LeftClick.performed += OnLeftClick;

            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Player.LeftClick.performed -= OnLeftClick;
            playerControls.Disable();
        }

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            leftClick = true;
        }

        private void Update()
        {
            HandleInputActions();
        }

        private void HandleInputActions()
        {
            HandleLeftClickAction();
        }

        private void HandleLeftClickAction()
        {
            if (leftClick)
            {
                leftClick = false;

                if (player == null)
                    return;

                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                // Двигаемся только если луч во что-то попал
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    movementPosition = hit.point;
                    player.playerLocomotionManager.MovePlayerToPosition(movementPosition);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (movementPosition == Vector3.zero)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(movementPosition, 0.2f);
        }
    }
}