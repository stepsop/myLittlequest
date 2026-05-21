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
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        // 2D‑проверка
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layerMask);

        IHoverable newHovered = null;
        if (hit != null)
            newHovered = hit.GetComponent<IHoverable>();

        // Смена объекта под курсором
        if (newHovered != currentHovered)
        {
            if (currentHovered != null)      // уход с предыдущего
                currentHovered.OnMouseExit();

            if (newHovered != null)          // вход на новый
                newHovered.OnMouseEnter();

            currentHovered = newHovered;
        }
    }
}
