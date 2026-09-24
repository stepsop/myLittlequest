using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseInteractDetector : MonoBehaviour
{
    public static MouseInteractDetector Instance { get; private set; }

    [Header("Настройки")]
    [SerializeField]
    private LayerMask layerMask = Physics2D.DefaultRaycastLayers;

    private Camera cam;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    public void TryInteractUnderCursor()
    {
        Debug.Log("[Interact] TryInteractUnderCursor called");

        Ray ray = cam.ScreenPointToRay(GameInput.GetMousePosition());

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            ray.origin,
            ray.direction,
            Mathf.Infinity,
            layerMask
        );

        Debug.Log($"[Interact] ray hits: {hits.Length}");

        // Идём от ближайшего объекта к самому дальнему.
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D collider = hits[i].collider;

            if (collider == null)
                continue;

            IInteractable interactable =
                collider.GetComponent<IInteractable>() ??
                collider.GetComponentInParent<IInteractable>();

            Debug.Log(
                $"[Interact] hit {collider.name}, " +
                $"IInteractable: {interactable != null}, " +
                $"CanInteract: {interactable?.CanInteract()}"
            );

            if (interactable != null && interactable.CanInteract())
            {
                interactable.Interact();
                return;
            }
        }
    }
}