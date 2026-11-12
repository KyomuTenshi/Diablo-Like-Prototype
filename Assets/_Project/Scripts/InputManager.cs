using System;
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

            playerControls.Player.LeftClick.performed += i => leftClick = true;

            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
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

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                    movementPosition = hit.point;

                player.playerLocomotionManager.MovePlayerToPosition(movementPosition);
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