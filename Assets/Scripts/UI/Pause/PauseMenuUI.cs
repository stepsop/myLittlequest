using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuPanel;
    public Button menuButton;

    [Header("Кнопки меню")]
    public Button settingsButton;
    public Button saveLoadButton;
    public Button exitButton;

    [Header("Панели")]
    public GameObject settingsPanel;
    public GameObject saveLoadPanel;

    private PlayerInputActions input;

    private void Awake()
    {
        input = new PlayerInputActions();
        input.Enable();
    }

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        menuButton.gameObject.SetActive(false);

        menuButton.onClick.AddListener(ToggleMenu);
        settingsButton.onClick.AddListener(OpenSettings);
        saveLoadButton.onClick.AddListener(OpenSaveLoad);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void Update()
    {
        if (input.Player.Menu.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        bool isOpen = !pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(isOpen);
        GameState.IsMenuOpen = isOpen;
    }

    public void CloseMenu()
    {
        pauseMenuPanel.SetActive(false);
        GameState.IsMenuOpen = false;
    }

    public void SetMenuButtonActive(bool active)
    {
        menuButton.gameObject.SetActive(active);
    }

    private void OpenSettings()
    {
        // TODO: открыть панель настроек
        Debug.Log("Настройки");
    }

    private void OpenSaveLoad()
    {
        // TODO: открыть панель сохранения
        Debug.Log("Сохранение");
    }

    private void ExitGame()
    {
        Debug.Log("Выход из игры");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}