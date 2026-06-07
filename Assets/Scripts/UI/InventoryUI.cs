using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }
    private PlayerInputActions input;

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel; // Перетащи сюда InventoryPanel
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private UIItemSlot slotPrefab;
    [SerializeField] private int itemsPerPage = 6;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private PauseMenuUI pauseMenuUI; // Назначи в редакторе

    private int currentPage = 0;
    private Dictionary<ItemData, UIItemSlot> slots = new();

    // Открыт ли инвентарь сейчас
    private bool isOpen = false;

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
        input.Enable();

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        // Instance = this;
        // input = new PlayerInputActions();
        // input.Enable();
    }

    private void Start()
    {
        // В сцене кнопки перелистывания иногда "теряют" target в OnClick (UnityEvent),
        // и тогда нажатие визуально происходит, но метод NextPage/PrevPage не вызывается.
        // Чтобы не чинить руками YAML каждой сцены — страхуемся кодом.
        WirePaginationButtonsIfNeeded();
    }

    private void WirePaginationButtonsIfNeeded()
    {
        // Привязываем обработчики к кнопкам только если у них нет валидных persistent listeners.
        // Это важно: если в инспекторе всё настроено корректно — мы не добавим второй обработчик
        // и не получим двойной перелист.
        TryWireButton(nextButton, NextPage);
        TryWireButton(prevButton, PrevPage);
    }

    private static void TryWireButton(GameObject buttonObject, UnityEngine.Events.UnityAction action)
    {
        if (buttonObject == null) return;

        var button = buttonObject.GetComponent<Button>();
        if (button == null) return;

        // Persistent listeners — это те, что видны в инспекторе (UnityEvent).
        // Проблема конкретно у тебя была в том, что listener есть, но target = null,
        // то есть Unity нечего вызывать.
        int persistentCount = button.onClick.GetPersistentEventCount();
        bool hasValidPersistentTarget = false;
        for (int i = 0; i < persistentCount; i++)
        {
            if (button.onClick.GetPersistentTarget(i) != null)
            {
                hasValidPersistentTarget = true;
                break;
            }
        }

        if (!hasValidPersistentTarget)
            button.onClick.AddListener(action);
    }

    private void Update()
    {


        if (input.Player.OpenInventory.WasPressedThisFrame())
        {
            Debug.Log("Кнопка I нажата!");
            if (GameState.IsDialogueOpen) return;
            if (GameState.IsMenuOpen) return; // Не открывать инвентарь если меню открыто
            ToggleInventory();
        }
    }

    /// <summary>
    /// Переключает инвентарь — если открыт закрывает, если закрыт открывает
    /// Toggle — это паттерн переключения состояния
    /// </summary>
    public void ToggleInventory()
    {
        isOpen = !isOpen; // Инвертируем состояние
        inventoryPanel.SetActive(isOpen);
        GameState.IsInventoryOpen = isOpen;

        // Управляем видимостью кнопки меню
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetMenuButtonActive(isOpen);

            // Если закрыли инвентарь — закрываем pausePanel
            if (!isOpen)
                pauseMenuUI.CloseMenu();
        }

        // Если открыли — обновляем UI чтобы показать актуальные предметы
        if (isOpen)
            RefreshUI(InventoryManager.Instance.Items);
    }

    public void RefreshUI(List<InventoryManager.ItemStack> items)
    {
        // Если контейнер уничтожен — выходим
        if (itemsContainer == null) return;

        foreach (Transform child in itemsContainer)
            Destroy(child.gameObject);

        slots.Clear();

        int totalPages = Mathf.CeilToInt((float)items.Count / itemsPerPage);
        totalPages = Mathf.Max(totalPages, 1);

        if (currentPage >= totalPages)
            currentPage = totalPages - 1;

        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, items.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            InventoryManager.ItemStack stack = items[i];
            if (slotPrefab == null)
            {
                Debug.LogError("slotPrefab НЕ назначен!");
                return;
            }

            UIItemSlot slot = Instantiate(slotPrefab, itemsContainer);
            slot.Setup(stack.itemData, stack.amount);
            slots[stack.itemData] = slot;
        }

        UpdatePageButtons(totalPages);
    }

    private void UpdatePageButtons(int totalPages)
    {
        nextButton.SetActive(currentPage < totalPages - 1);
        prevButton.SetActive(currentPage > 0);
    }

    public void Highlight(ItemData item)
    {
        foreach (var s in slots.Values)
            s.SetSelected(false);

        if (slots.ContainsKey(item))
            slots[item].SetSelected(true);
    }

    public void ClearSelection()
    {
        foreach (var s in slots.Values)
            s.SetSelected(false);
    }

    public void NextPage()
    {
        currentPage++;
        RefreshUI(InventoryManager.Instance.Items);
    }

    public void PrevPage()
    {
        currentPage--;
        RefreshUI(InventoryManager.Instance.Items);
    }
    private void OnDestroy()
    {
        input?.Disable();
    }
}