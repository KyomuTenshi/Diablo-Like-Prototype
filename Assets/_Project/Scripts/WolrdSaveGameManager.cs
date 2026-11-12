using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        // Singleton: доступ из любого скрипта через WorldSaveGameManager.instance
        public static WorldSaveGameManager instance;

        [Header("World Index")]
        // Номер сцены мира в Build Settings
        [SerializeField] int worldSceneIndex = 1;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                // Один менеджер уже есть, дубликат удаляем
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Менеджер переживёт смену сцены
            DontDestroyOnLoad(gameObject);
        }

        public void StartNewGame()
        {
            SceneManager.LoadScene(worldSceneIndex);
        }
    }
}