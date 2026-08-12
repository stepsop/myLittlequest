using UnityEngine;

public class InteractionHint : MonoBehaviour, IHoverable
{
    [SerializeField] private Transform hintPoint; // сюда кидаешь дочерний объект "Hint"

    // Текст подсказки — берём из PickupItem или имени объекта
    private string hintText;

    private void Awake()
    {
        RefreshHintText();
    }

    private void Start()
    {
        RefreshHintText();
    }

    public void OnMouseEnter()
    {
        InteractionHintUI hintUI = InteractionHintUI.GetOrFindInstance();
        if (hintUI == null)
        {
            Debug.LogWarning("InteractionHintUI не найден в сцене. Добавь HintCanvas или UIRoot с InteractionHintUI.", this);
            return;
        }

        // Если hintPoint не назначен — используем transform самого объекта
        Transform point = hintPoint != null ? hintPoint : transform;
        hintUI.Show(hintText, point);
    }

    public void OnMouseExit()
    {
        InteractionHintUI.GetOrFindInstance()?.Hide();
    }

    // Публичные методы для ручного вызова (если нужно)
    public void Show() => OnMouseEnter();
    public void Hide() => OnMouseExit();

    private void RefreshHintText()
    {
        PickupItem pickup = GetComponent<PickupItem>();
        hintText = (pickup?.itemData?.itemName) ?? gameObject.name;
    }
}