using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class UIItemSlot : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Image selectionFrame;
    [SerializeField] private TextMeshProUGUI amountText;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipItemName;

    private ItemData item;
    private int currentAmount;

    private bool isDragging = false;

    private static GameObject dragIcon;
    private static UIItemSlot dragSource;

    public ItemData Item => item;

    public void Setup(ItemData data, int amount)
    {
        item = data;
        currentAmount = amount;
        icon.sprite = data.icon;
        amountText.text = amount > 1 ? amount.ToString() : "";
        selectionFrame.enabled = false;

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    // --- TOOLTIP ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || tooltipPanel == null) return;
        if (tooltipItemName != null)
            tooltipItemName.text = item.itemName;
        tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    // --- КЛИКИ ---

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;
        if (isDragging) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Левая кнопка — выбрать предмет
            InventoryManager.Instance.SelectItem(item);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Правая кнопка нажата");// Правая кнопка — осмотреть предмет
            InspectItem();
        }
    }

    private void InspectItem()
    {
        Debug.Log("in method InspectItem ");
        if (item == null) return;

        // Открываем панель описания
        ItemInspectPanel.Instance?.Show(item);

        // Трансформация предмета при осмотре
        if (item.inspectTransformTo != null)
        {
            InventoryManager.Instance.RemoveItem(item);
            InventoryManager.Instance.AddItem(item.inspectTransformTo);
        }
    }

    // public void OnClick()
    // {
    // Важно: этот метод вызывается Unity UI Button'ом (onClick) из префаба слота.
    //
    // Но этот же класс также реализует IPointerClickHandler (OnPointerClick),
    // где мы уже обрабатываем ЛКМ/ПКМ и делаем SelectItem/InspectItem.
    //
    // Если оставить SelectItem и тут, и в OnPointerClick, то при клике ЛКМ часто происходит:
    // 1) OnPointerClick -> SelectItem(item)  (выбрали)
    // 2) Button.onClick -> OnClick -> SelectItem(item) (сразу же сняли, потому что SelectItem = toggle)
    //
    // В итоге визуально кажется, что выбор предмета "перестал работать".
    // Поэтому onClick-обработчик намеренно оставляем пустым и используем OnPointerClick как единую точку правды.
    // }

    public void SetSelected(bool value)
    {
        selectionFrame.enabled = value;
    }

    // --- DRAG & DROP ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        // Не начинаем drag правой кнопкой
        if (eventData.button != PointerEventData.InputButton.Left) return;
        isDragging = true;

        dragSource = this;

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(GetRootCanvas().transform, false);
        dragIcon.transform.SetAsLastSibling();

        var img = dragIcon.AddComponent<Image>();
        img.sprite = icon.sprite;
        img.raycastTarget = false;

        dragIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
        icon.color = new Color(1, 1, 1, 0.4f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        icon.color = Color.white;

        if (dragIcon != null)
        {
            Destroy(dragIcon);
            dragIcon = null;
        }

        dragSource = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (dragSource == null || dragSource == this) return;
        if (dragSource.item == item) return;

        dragSource.ResetDragVisual();
        CombineManager.Instance.TryCombine(dragSource.item, item);
    }

    public void ResetDragVisual()
    {
        icon.color = Color.white;
        if (dragIcon != null)
        {
            Destroy(dragIcon);
            dragIcon = null;
        }
    }

    private Canvas GetRootCanvas()
    {
        var canvas = GetComponentInParent<Canvas>();
        while (canvas.transform.parent != null &&
               canvas.transform.parent.GetComponentInParent<Canvas>() != null)
            canvas = canvas.transform.parent.GetComponentInParent<Canvas>();
        return canvas;
    }
}