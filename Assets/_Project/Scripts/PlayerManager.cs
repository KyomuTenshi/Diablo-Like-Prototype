using UnityEngine;

namespace SG
{
    public class PlayerManager : MonoBehaviour
    {
        private void Awake()
        {
            // Игрок переходит из меню в мир вместе с загрузкой новой сцены
            DontDestroyOnLoad(gameObject);
        }
    }
}