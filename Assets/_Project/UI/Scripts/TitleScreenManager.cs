using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace SG
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Press Start Menu")]
        // CanvasGroup позволяет менять прозрачность сразу у всех элементов меню
        [SerializeField] CanvasGroup pressStartMenuCanvasGroup;
        // Храним ссылку на корутину, чтобы можно было остановить её, если она уже идёт
        private Coroutine fadeOutPressStartMenuCoroutine;
        [SerializeField] float fadeTime = 1.0f;

        [Header("Character Menu")]
        [SerializeField] CanvasGroup characterMenuCanvasGroup;
        private Coroutine fadeInCharacterMenuCoroutine;
        [SerializeField] GameObject[] characterMenuButton;

        // Текущий клиент становится хостом, к нему смогут подключаться другие игроки
        public void StartGame()
        {
            NetworkManager.Singleton.StartHost();
            FadeOutPressStartMenu();
            FadeInCharacterMenu();
        }

        private void FadeOutPressStartMenu()
        {
            // Не запускаем вторую корутину поверх первой
            if (fadeOutPressStartMenuCoroutine != null)
                StopCoroutine(fadeOutPressStartMenuCoroutine);

            fadeOutPressStartMenuCoroutine = StartCoroutine(FadeOutPressStartMenuCoroutine(fadeTime));
        }

        // Корутина: плавно уменьшает alpha от текущего значения до 0 за duration секунд
        private IEnumerator FadeOutPressStartMenuCoroutine(float duration)
        {
            float timeElapsed = 0f;
            float startingAlpha = pressStartMenuCanvasGroup.alpha;

            while (timeElapsed < duration)
            {
                // timeElapsed / duration даёт число от 0 до 1 — доля пройденного времени
                pressStartMenuCanvasGroup.alpha = Mathf.Lerp(startingAlpha, 0f, timeElapsed / duration);
                timeElapsed += Time.deltaTime; // время, прошедшее с прошлого кадра
                yield return null;             // ждём следующий кадр
            }

            // Lerp не доходит ровно до 0 на последнем кадре, поэтому ставим вручную
            pressStartMenuCanvasGroup.alpha = 0f;
        }

        // Вызывается кнопкой New Game
        public void StartNewGame()
        {
            WorldSaveGameManager.instance.StartNewGame();
        }
        
        private void FadeInCharacterMenu()
        {
            if (fadeInCharacterMenuCoroutine != null)
                StopCoroutine(fadeInCharacterMenuCoroutine);

            fadeInCharacterMenuCoroutine = StartCoroutine(FadeInCharacterMenuCoroutine(fadeTime));

            // Включаем кнопки меню, пропуская пустые (не назначенные в Inspector) ячейки
            for (int i = 0; i < characterMenuButton.Length; i++)
            {
                if (characterMenuButton[i] == null)
                    continue;

                characterMenuButton[i].SetActive(true);
            }
        }

        // Корутина: плавно увеличивает alpha от 0 до 1 за duration секунд
        private IEnumerator FadeInCharacterMenuCoroutine(float duration)
        {
            float timeElapsed = 0f;
            float startingAlpha = 0f;

            while (timeElapsed < duration)
            {
                // Здесь Lerp идёт от 0 к 1 — меню появляется, а не исчезает
                characterMenuCanvasGroup.alpha = Mathf.Lerp(startingAlpha, 1f, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            characterMenuCanvasGroup.alpha = 1f;
        }
    }
}