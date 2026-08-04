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
    private ItemData currentItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameObject.SetActive(true);
        input = new PlayerInputActions();
        input.Player.Enable();
       

        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
      
        if (panel != null && panel.activeSelf && input.Player.Menu.WasPressedThisFrame())
        {
            Close();
        }
    }

    public void Show(ItemData item)
    {
        if (item == null) return;

        if (panel.activeSelf && currentItem == item)
        {
            Close();
            return;
        }

        currentItem = item;
        panel.SetActive(true);

        if (itemIcon != null) itemIcon.sprite = item.icon;
        if (itemName != null) itemName.text = item.itemName;
        if (description != null)
        {
            description.text = !string.IsNullOrEmpty(item.inspectPhrase)
                ? item.inspectPhrase
                : "Нет описания.";
        }

        GameState.IsInspecting = true;
        SpeechBubble.Instance?.Show(item.inspectPhrase, item.inspectAudio);
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        GameState.IsInspecting = false;
        currentItem = null;
    }
    private void OnDestroy()
    {
        input?.Player.Disable();
    }
}