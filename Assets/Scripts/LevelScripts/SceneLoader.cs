using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // Singleton — один SceneLoader на всю игру
    public static SceneLoader Instance { get; private set; }

    [Header("Fade эффект")]
    // Чёрный Image который перекрывает экран при переходе
    [SerializeField] private Image fadeImage;

    // Сколько секунд длится затемнение
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        // SceneLoader должен быть единственным на всю игру.
        // Важно: MissingReferenceException появляется, когда static Instance указывает на уже уничтоженный объект.
        // Поэтому:
        // - уничтожаем только ДУБЛИ (Instance != this)
        // - в OnDestroy сбрасываем Instance, если уничтожили текущий экземпляр
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Этот объект не уничтожается при смене сцены
        // Именно поэтому fade-эффект работает плавно между сценами
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        // Если Unity уничтожил этот объект (например, из-за дублей или ручного удаления),
        // нельзя оставлять Instance указывать на "destroyed" объект — это и вызывает MissingReferenceException.
        if (Instance == this)
            Instance = null;
    }

    // Главный метод — вызывается из SceneTransition
    // sceneName — это название файла сцены без расширения
    public void LoadScene(string sceneName)
    {
        // Защита: если объект по какой-то причине уничтожен/выключен, не пытаемся стартовать корутину.
        // Лучше лог + безопасный выход, чем падение игры.
        if (!this || !isActiveAndEnabled)
        {
            Debug.LogError($"SceneLoader.LoadScene({sceneName}) вызван, но SceneLoader не активен или уничтожен.");
            return;
        }

        if (fadeImage == null)
        {
            Debug.LogError("SceneLoader: fadeImage не назначен в инспекторе. Переход невозможен без fadeImage.");
            return;
        }

        // Запускаем корутину — она делает:
        // затемнение → загрузка сцены → осветление
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Блокируем управление пока идёт переход
        GameState.IsTransitioning = true;

        // Шаг 1 — плавно затемняем экран
        yield return StartCoroutine(Fade(0f, 1f));

        // Шаг 2 — загружаем новую сцену
        // LoadSceneMode.Single — выгружает старую сцену и загружает новую
        SceneManager.LoadScene(sceneName);

        // Шаг 3 — ждём один кадр чтобы сцена успела загрузиться
        yield return null;

        // Шаг 4 — говорим камере найти новый Floor
        // Camera.main может быть null (нет MainCamera) или CameraFollow может отсутствовать.
        // Это не критично для перехода, поэтому делаем null-safe.
        if (Camera.main != null && Camera.main.TryGetComponent(out CameraFollow follow))
            follow.FindBounds();

        // Шаг 5 — плавно осветляем экран
        yield return StartCoroutine(Fade(1f, 0f));

        // Разблокируем управление
        GameState.IsTransitioning = false;
    }

    // Корутина плавного изменения прозрачности
    // from — начальная прозрачность (0 = прозрачный, 1 = чёрный)
    // to — конечная прозрачность
    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // Lerp плавно меняет значение от from до to
            // elapsed / fadeDuration — это прогресс от 0 до 1
            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);

            // Ждём следующий кадр
            yield return null;
        }

        // Гарантируем точное конечное значение
        fadeImage.color = new Color(color.r, color.g, color.b, to);
    }
}