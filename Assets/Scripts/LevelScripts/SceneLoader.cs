using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Отвечает ТОЛЬКО за загрузку сцены и порядок действий вокруг неё.
// Fade — в FadeController. Спавн игрока — в PlayerSpawnController.
// Никаких статических флагов между кадрами — весь порядок здесь, явно.
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private FadeController fadeController;

    private bool isLoading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // targetSpawnID — куда поставить игрока в новой сцене.
    // Передаём явным параметром, а не через статику — исключает гонки.
    public void LoadScene(string sceneName, int targetSpawnID)
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneRoutine(sceneName, targetSpawnID));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, int targetSpawnID)
    {
        isLoading = true;
        GameState.IsTransitioning = true;

        // Fade Out
        if (fadeController != null)
            yield return StartCoroutine(fadeController.Fade(0f, 1f));

        // Загружаем сцену
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOperation.isDone)
            yield return null;

        // Ждём кадр — GameManager успевает заспавнить UIRoot и т.д.
        yield return null;

        // Спавним игрока НА НУЖНОМ SpawnPoint.
        // Это явный вызов, а не флаг + угадывание порядка Start() —
        // поэтому 100% гарантия, что игрок появится в правильном месте.
        if (PlayerSpawnManager.Instance != null)
            PlayerSpawnManager.Instance.SpawnAtID(targetSpawnID);
        else
            Debug.LogError("SceneLoader: PlayerSpawnManager.Instance == null. Добавь его в GameManager prefab.");

        // Сбрасываем состояния UI
        GameState.IsMenuOpen = false;
        GameState.IsDialogueOpen = false;
        GameState.IsInspecting = false;

        // Fade In
        if (fadeController != null)
            yield return StartCoroutine(fadeController.Fade(1f, 0f));

        GameState.IsTransitioning = false;
        isLoading = false;
    }
}