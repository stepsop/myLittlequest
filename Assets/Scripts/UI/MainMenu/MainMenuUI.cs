using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Контроллер главного меню.
// Отвечает только за навигацию между панелями и запуск игры.
// Логика сохранений — в SaveManager (будет отдельно).
public class MainMenuUI : MonoBehaviour
{
    [Header("Панели")]
    [SerializeField] private GameObject menuPanel;  // Главная панель
    [SerializeField] private GameObject playPanel;  // Панель выбора игры

    [Header("Кнопки главной панели")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Кнопки панели игры")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton; // Видна только если есть сохранение
    [SerializeField] private Button backButton;

    // Ключ для проверки наличия сохранения в PlayerPrefs
    // PlayerPrefs — простое хранилище ключ/значение, переживает перезапуск игры
    private const string SaveKey = "HasSave";

    private void Start()
    {
        // Показываем главную панель, скрываем остальные
        menuPanel.SetActive(true);
        playPanel.SetActive(false);

        // Подписываем кнопки на методы
        playButton.onClick.AddListener(OpenPlayPanel);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);

        newGameButton.onClick.AddListener(StartNewGame);
        continueButton.onClick.AddListener(ContinueGame);
        backButton.onClick.AddListener(ClosePlayPanel);

        // Показываем кнопку "Продолжить" только если есть сохранение
        // PlayerPrefs.HasKey проверяет существует ли ключ в хранилище
        continueButton.gameObject.SetActive(PlayerPrefs.HasKey(SaveKey));
    }

    // Открыть панель выбора — новая игра или продолжить
    private void OpenPlayPanel()
    {
        menuPanel.SetActive(false);
        playPanel.SetActive(true);
    }

    // Вернуться к главной панели
    private void ClosePlayPanel()
    {
        playPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void StartNewGame()
    {
        // Сбрасываем сохранение при новой игре
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Загружаем первый уровень
        // Индекс 1 — потому что MainMenu = 0, Level1 = 1 в Build Settings
        SceneManager.LoadScene(1);
    }

    private void ContinueGame()
    {
        // Загружаем сцену из сохранения — пока просто Level1
        // Когда добавим SaveManager — будем загружать нужную сцену
        SceneManager.LoadScene(1);
    }

    private void OpenSettings()
    {
        // TODO — подключим к панели настроек позже
        Debug.Log("Настройки — будет позже");
    }

    private void ExitGame()
    {
        Debug.Log("Выход");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}