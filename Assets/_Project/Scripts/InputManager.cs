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

        [Header("WASD Input")]
        [SerializeField] bool enableWASDMovement = false;
        [SerializeField] Vector2 movementInput;
        // Запоминаем, что персонажа вели через WASD, чтобы остановить его один раз при отпускании клавиш
        private bool wasMovingWithWASD = false;

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

            playerControls.Player.LeftClick.performed += OnLeftClick;
            playerControls.Player.WASD.performed += OnWASD;

            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Player.LeftClick.performed -= OnLeftClick;
            playerControls.Player.WASD.performed -= OnWASD;
            playerControls.Disable();

            // Сбрасываем ввод, чтобы после возврата из меню персонаж не побежал сам
            movementInput = Vector2.zero;
            wasMovingWithWASD = false;
        }

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            leftClick = true;
        }

        // Срабатывает при каждом изменении WASD, в том числе когда клавиши отпущены (придёт Vector2.zero)
        private void OnWASD(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            HandleInputActions();
        }

        private void HandleInputActions()
        {
            HandleLeftClickAction();
            HandleWASDMovement();
        }

        private void HandleLeftClickAction()
        {
            if (leftClick)
            {
                leftClick = false;

                if (player == null)
                    return;

                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    movementPosition = hit.point;
                    player.playerLocomotionManager.MovePlayerToPosition(movementPosition);
                }
            }
        }

        private void HandleWASDMovement()
        {
            // if (!enableWASDMovement)
            //     return;

            // Игрок ещё не создан (например, сразу после загрузки сцены)
            if (player == null)
                return;

            if (movementInput.sqrMagnitude < 0.001f)
            {
                // Клавиши отпущены: останавливаем персонажа один раз.
                // Если делать это каждый кадр, движение по клику мышью сразу отменится
                if (wasMovingWithWASD)
                {
                    wasMovingWithWASD = false;
                    player.playerLocomotionManager.MovePlayerToPosition(player.transform.position);
                }
                return;
            }

            wasMovingWithWASD = true;

            // Направления камеры вдоль земли (y = 0, чтобы не идти вверх или вниз по взгляду камеры)
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            // W/S двигают вдоль forward камеры, A/D вдоль right
            Vector3 moveDirection = (cameraForward * movementInput.y) + (cameraRight * movementInput.x);

            // Цель: точка чуть впереди персонажа, агент сам к ней побежит
            player.playerLocomotionManager.MovePlayerToPosition(player.transform.position + moveDirection);
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