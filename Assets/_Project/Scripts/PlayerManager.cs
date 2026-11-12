using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    public class PlayerManager : NetworkBehaviour
    {
        [HideInInspector] public NavMeshAgent navMeshAgent;

        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;

        private void Awake()
        {
            // Игрок переходит из меню в мир вместе с загрузкой новой сцены
            DontDestroyOnLoad(gameObject);

            navMeshAgent = GetComponentInChildren<NavMeshAgent>();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                InputManager.instance.player = this;
            }
        }
    }
}