using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInspectPanel : MonoBehaviour
{
    public static ItemInspectPanel Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI description;

    private PlayerInputActions input;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        input = new PlayerInputActions();
        input.Enable();

        panel.SetActive(false);
    }

    private void Update()
    {
        // Закрыть по Escape
        if (panel.activeSelf && input.Player.Menu.WasPressedThisFrame())
            Close();
    }

    public void Show(ItemData item)
    {
        if (item == null) return;

        itemIcon.sprite = item.icon;
        itemName.text = item.itemName;
        description.text = !string.IsNullOrEmpty(item.inspectPhrase)
            ? item.inspectPhrase
            : "Нет описания.";

        panel.SetActive(true);
        GameState.IsInspecting = true;

        // Показываем SpeechBubble одновременно
        SpeechBubble.Instance?.Show(item.inspectPhrase, item.inspectAudio);
    }

    public void Close()
    {
        panel.SetActive(false);
        GameState.IsInspecting = false;
    }
}