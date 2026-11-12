using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    public class PlayerManager : NetworkBehaviour
    {
        [HideInInspector] public NavMeshAgent navMeshAgent;
        [HideInInspector] public Animator animator;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;

        private void Awake()
        {
            // Игрок переходит из меню в мир вместе с загрузкой новой сцены
            DontDestroyOnLoad(gameObject);

            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
        }

        private void Update()
        {
            if (IsOwner)
            {
                // Владелец двигает персонажа сам и публикует позицию для остальных
                playerLocomotionManager.HandleMovement();

                playerNetworkManager.networkPosition.Value = transform.position;
                playerNetworkManager.networkRotation.Value = transform.rotation;
            }
            else
            {
                // Чужие копии игрока плавно догоняют позицию владельца
                transform.position = Vector3.SmoothDamp(transform.position, playerNetworkManager.networkPosition.Value, ref playerNetworkManager.networkPositionVelocity, playerNetworkManager.networkSmothing);
                transform.rotation = Quaternion.Lerp(transform.rotation, playerNetworkManager.networkRotation.Value, Time.deltaTime / playerNetworkManager.networkSmothing);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                InputManager.instance.player = this;
            }

            playerNetworkManager.isMoving.OnValueChanged += playerNetworkManager.OnIsMovingChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            playerNetworkManager.isMoving.OnValueChanged -= playerNetworkManager.OnIsMovingChanged;
        }
    }
}