using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseHoverDetector : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Какой слой проверять (по умолчанию Default)")]
    public LayerMask layerMask = Physics2D.DefaultRaycastLayers;

    private Camera cam;
    private IHoverable currentHovered;   // объект, над которым сейчас курсор

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!IsHoverableAlive(currentHovered))
            currentHovered = null;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        // 2D‑проверка
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layerMask);

        IHoverable newHovered = null;
        if (hit != null)
            newHovered = hit.GetComponent<IHoverable>();
        if (!IsHoverableAlive(newHovered))
            newHovered = null;

        // Смена объекта под курсором
        if (newHovered != currentHovered)
        {
            if (IsHoverableAlive(currentHovered))      // уход с предыдущего
                currentHovered.OnMouseExit();

            if (IsHoverableAlive(newHovered))          // вход на новый
                newHovered.OnMouseEnter();

            currentHovered = newHovered;
        }
    }

    private void OnDisable()
    {
        if (IsHoverableAlive(currentHovered))
            currentHovered.OnMouseExit();

        currentHovered = null;
    }

    private static bool IsHoverableAlive(IHoverable hoverable)
    {
        if (hoverable == null) return false;

        Object unityObject = hoverable as Object;
        return unityObject == null || unityObject != null;
    }
}
