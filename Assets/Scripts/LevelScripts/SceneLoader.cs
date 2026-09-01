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
        StartCoroutine(LoadSceneRoutine(sceneName, () =>
        {
            if (PlayerSpawnManager.Instance != null)
                PlayerSpawnManager.Instance.SpawnAtID(targetSpawnID);
            else
                Debug.LogError("SceneLoader: PlayerSpawnManager.Instance == null. Добавь его в GameManager prefab.");
        }));
    }

    // Используется при загрузке сохранения — вместо SpawnPoint с ID
    // ставим игрока на сырые координаты из SaveData.
    public void LoadSceneAtPosition(string sceneName, Vector3 position)
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneRoutine(sceneName, () =>
        {
            if (PlayerSpawnManager.Instance != null)
                PlayerSpawnManager.Instance.SpawnAtPosition(position);
            else
                Debug.LogError("SceneLoader: PlayerSpawnManager.Instance == null. Добавь его в GameManager prefab.");
        }));
    }

    // Общая корутина: fade out → загрузка сцены → спавн игрока (через колбэк) → fade in.
    // spawnAction решает КАК именно поставить игрока — по SpawnPoint ID или по координатам.
    private IEnumerator LoadSceneRoutine(string sceneName, System.Action spawnAction)
    {
        isLoading = true;
        GameState.Current = UIState.Transitioning;

        // Fade Out
        if (fadeController != null)
            yield return StartCoroutine(fadeController.Fade(0f, 1f));

        // Загружаем сцену
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOperation.isDone)
            yield return null;

        // Ждём кадр — GameManager успевает заспавнить UIRoot и т.д.
        yield return null;

        // Спавним игрока — конкретный способ передан через spawnAction.
        spawnAction?.Invoke();

        // Сбрасываем состояния UI
        GameState.Current = UIState.None;
        // Fade In
        if (fadeController != null)
            yield return StartCoroutine(fadeController.Fade(1f, 0f));

        
        isLoading = false;
    }
}