using UnityEngine;
using System.Collections;

namespace SG
{
    public class PlayerLocomotionManager : MonoBehaviour
    {
        PlayerManager player;

        // Ссылка на корутину, чтобы остановить старую при выборе новой цели
        private Coroutine hasReachedDestinationCoroutine;

        private void Awake()
        {
            player = GetComponent<PlayerManager>();
        }

        public void HandleMovement()
        {
            SyncCharacterWithAgent();
        }

        public void MovePlayerToPosition(Vector3 newPosition)
        {
            player.navMeshAgent.SetDestination(newPosition);

            if (hasReachedDestinationCoroutine != null)
                StopCoroutine(hasReachedDestinationCoroutine);

            player.playerNetworkManager.isMoving.Value = true;
            hasReachedDestinationCoroutine = StartCoroutine(HasReachedDestinationCoroutine());
        }

        // Ждёт, пока персонаж дойдёт до цели
        private IEnumerator HasReachedDestinationCoroutine()
        {
            while (player.navMeshAgent.pathPending)
            {
                yield return null;
            }

            while (player.navMeshAgent.remainingDistance > player.navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            player.playerNetworkManager.isMoving.Value = false;
        }

        // Агент бежит по NavMesh в своём дочернем объекте, а персонаж (родитель) должен за ним следовать
        private void SyncCharacterWithAgent()
        {
            // Сначала переносим персонажа туда, где сейчас агент
            player.transform.position = player.navMeshAgent.transform.position;

            if (player.playerNetworkManager.isMoving.Value)
                player.transform.rotation = player.navMeshAgent.transform.rotation;

            // Затем возвращаем агента в центр персонажа
            player.navMeshAgent.transform.localPosition = Vector3.zero;
            player.navMeshAgent.transform.localRotation = Quaternion.identity;
        }
    }
}