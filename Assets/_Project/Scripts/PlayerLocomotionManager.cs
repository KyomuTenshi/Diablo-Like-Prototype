using UnityEngine;
using System.Collections;

namespace SG
{
    public class PlayerLocomotionManager : MonoBehaviour
    {
        PlayerManager player;

        // Ссылка на корутину, чтобы остановить старую при выборе новой цели
        private Coroutine hasReachedDestinationCoroutine;
        // Скорость поворота в градусах в секунду
        [SerializeField] float rotationSpeed = 360;

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

        // Агент бежит по NavMesh в дочернем объекте, а персонаж (родитель) следует за ним
        private void SyncCharacterWithAgent()
        {
            // Переносим персонажа туда, где сейчас агент
            player.transform.position = player.navMeshAgent.transform.position;

            if (player.playerNetworkManager.isMoving.Value)
            {
                // steeringTarget — ближайшая точка пути, к которой агент идёт прямо сейчас
                Vector3 desiredDirection = player.navMeshAgent.steeringTarget - player.transform.position;
                // Обнуляем высоту, чтобы персонаж не наклонялся вверх или вниз
                desiredDirection.y = 0;

                // Проверяем после обнуления y, иначе LookRotation может получить нулевой вектор
                if (desiredDirection.sqrMagnitude > 0.001f)
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection);

                    // Поворачиваем плавно, не быстрее rotationSpeed градусов в секунду
                    player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
                }
            }

            // Возвращаем агента в центр персонажа, чтобы он не «убегал» от родителя
            player.navMeshAgent.transform.localPosition = Vector3.zero;
            player.navMeshAgent.transform.localRotation = Quaternion.identity;
        }
    }
}