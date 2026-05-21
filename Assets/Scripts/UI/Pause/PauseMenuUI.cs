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
            // Если открыт инспектор - закрываем его и НЕ открываем меню паузы
            if (GameState.IsInspecting)
            {
                ItemInspectPanel.Instance?.Close();
                return; // Выходим, чтобы меню паузы не открывалось
            }
            
            // Если инспектор закрыт - открываем/закрываем меню паузы
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
        Debug.Log("Настройки");
    }

    private void OpenSaveLoad()
    {
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