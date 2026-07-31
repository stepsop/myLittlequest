using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseHoverDetector : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Какой слой проверять (по умолчанию Default)")]
    public LayerMask layerMask = Physics2D.DefaultRaycastLayers;

    private readonly Collider2D[] hoverHits = new Collider2D[16];
    private Camera cam;
    private IHoverable currentHovered;   // объект, над которым сейчас курсор

    private ContactFilter2D contactFilter;

    private void Awake()
    {
        cam = GetComponent<Camera>();


        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(layerMask);
        contactFilter.useTriggers = true;
    }

    private void Update()
    {
        if (!IsHoverableAlive(currentHovered))
            currentHovered = null;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        IHoverable newHovered = FindHoverableAt(mouseWorldPos);

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

    private IHoverable FindHoverableAt(Vector3 worldPosition)
    {
        int hitCount = Physics2D.OverlapPoint(worldPosition, contactFilter, hoverHits);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = hoverHits[i];
            if (hit == null)
                continue;

            IHoverable hoverable = hit.GetComponent<IHoverable>() ?? hit.GetComponentInParent<IHoverable>();
            if (IsHoverableAlive(hoverable))
                return hoverable;
        }

        return null;
    }

    private static bool IsHoverableAlive(IHoverable hoverable)
    {
        if (hoverable == null) return false;
        Object unityObject = hoverable as Object;
        return unityObject != null; // уничтоженный объект вернёт false
    }
}
