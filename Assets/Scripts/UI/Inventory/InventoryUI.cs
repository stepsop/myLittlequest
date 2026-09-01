using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }
    private PlayerInputActions input;

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel; 
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private UIItemSlot slotPrefab;
    [SerializeField] private int itemsPerPage = 6;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private PauseMenuUI pauseMenuUI; 

    [Header("Анимация")]
    [SerializeField] private InventorySlideAnimation slideAnimation; 

    private int currentPage = 0;
    private Dictionary<ItemData, UIItemSlot> slots = new();

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
    }

    private void Start()
    {
        WirePaginationButtonsIfNeeded();
    }

    private void WirePaginationButtonsIfNeeded()
    {
        TryWireButton(nextButton, NextPage);
        TryWireButton(prevButton, PrevPage);
    }

    private static void TryWireButton(GameObject buttonObject, UnityEngine.Events.UnityAction action)
    {
        if (buttonObject == null) return;

        var button = buttonObject.GetComponent<Button>();
        if (button == null) return;

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
            if (GameState.IsDialogueOpen) return;
            if (GameState.IsMenuOpen) return;
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        GameState.Current = isOpen ? UIState.Inventory : UIState.None;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetMenuButtonActive(isOpen);

            if (!isOpen)
                pauseMenuUI.CloseMenu();
        }

        if (isOpen)
            RefreshUI(InventoryManager.Instance.Items);

        if (slideAnimation != null)
        {
            if (isOpen)
                slideAnimation.Show();
            else
                slideAnimation.Hide();
        }
    }

    public void RefreshUI(List<InventoryManager.ItemStack> items)
    {
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
        if (nextButton != null)
            nextButton.SetActive(currentPage < totalPages - 1);

        if (prevButton != null)
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