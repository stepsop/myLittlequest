using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

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
        // SaveManager.HasSave() проверяет существует ли сохранение
        continueButton.gameObject.SetActive(SaveManager.HasSave());
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
        // Удаляем сохранение из PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Очищаем состояние в памяти —
        // иначе инвентарь из прошлой сессии останется
        InventoryManager.Instance?.ClearInventory();
        PickupTracker.Instance?.LoadPickedUpItems(new List<string>());

        // Сбрасываем состояния всех NPC
        NPCState[] allStates = Resources.FindObjectsOfTypeAll<NPCState>();
        foreach (var state in allStates)
            state.Reset();

        SceneManager.LoadScene("Level1");

    }

    private void ContinueGame()
    {
        // Загружаем только если сохранение реально есть
        if (!SaveManager.HasSave()) return;
        SaveManager.Instance.Load();

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