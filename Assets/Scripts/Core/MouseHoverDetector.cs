using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseHoverDetector : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Какие слои участвуют в наведении")]
    public LayerMask layerMask = Physics2D.DefaultRaycastLayers;

    private Camera cam;
    private IHoverable currentHovered;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!IsHoverableAlive(currentHovered))
        {
            InteractionHintUI.GetOrFindInstance()?.Hide();
            currentHovered = null;
        }

        IHoverable newHovered = FindHoverableUnderMouse();

        if (newHovered != currentHovered)
        {
            if (IsHoverableAlive(currentHovered))
                currentHovered.OnMouseExit();

            if (IsHoverableAlive(newHovered))
                newHovered.OnMouseEnter();

            currentHovered = newHovered;
        }
    }

    private IHoverable FindHoverableUnderMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            ray.origin,
            ray.direction,
            Mathf.Infinity,
            layerMask
        );

        // RaycastAll возвращает попадания от ближайшего
        // объекта к самому дальнему.
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D collider = hits[i].collider;

            if (collider == null)
                continue;

            IHoverable hoverable =
                collider.GetComponent<IHoverable>() ??
                collider.GetComponentInParent<IHoverable>();

            if (IsHoverableAlive(hoverable))
                return hoverable;
        }

        return null;
    }

    private void OnDisable()
    {
        if (IsHoverableAlive(currentHovered))
            currentHovered.OnMouseExit();

        currentHovered = null;
    }

    private static bool IsHoverableAlive(IHoverable hoverable)
    {
        if (hoverable == null)
            return false;

        Object unityObject = hoverable as Object;

        return unityObject != null;
    }
}