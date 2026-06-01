using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Fade")]
    [SerializeField] private Image fadeImage;

    [SerializeField] private float fadeDuration = 0.5f;

    private bool isLoading = false;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // =====================================
    // ЗАГРУЗКА СЦЕНЫ
    // =====================================

    public void LoadScene(string sceneName)
    {
        if (isLoading)
            return;

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;

        GameState.IsTransitioning = true;

        // Fade Out
        yield return StartCoroutine(Fade(0f, 1f));

        // Загружаем сцену
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        // Ждём полной загрузки
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // ВАЖНО:
        // ждём ещё 1 кадр после загрузки
        yield return null;

        // Сбрасываем состояния
        GameState.IsTransitioning = false;
        GameState.IsMenuOpen = false;
        GameState.IsDialogueOpen = false;
        GameState.IsInspecting = false;

        // Fade In
        yield return StartCoroutine(Fade(1f, 0f));

        isLoading = false;

        Debug.Log("Transition complete");
    }

    // =====================================
    // FADE
    // =====================================

    private IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image не назначен.");

            yield break;
        }

        float elapsed = 0f;

        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);

            fadeImage.color = new Color(
                color.r,
                color.g,
                color.b,
                alpha
            );

            yield return null;
        }

        fadeImage.color = new Color(
            color.r,
            color.g,
            color.b,
            to
        );
    }
}