using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseInteractDetector : MonoBehaviour
{
    public static MouseInteractDetector Instance { get; private set; }

    [SerializeField] private LayerMask layerMask = Physics2D.DefaultRaycastLayers;

    private Camera cam;
    private readonly Collider2D[] hits = new Collider2D[32];
    private ContactFilter2D contactFilter;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(layerMask);
        contactFilter.useTriggers = true;
    }

    public void TryInteractUnderCursor()
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(GameInput.GetMousePosition());
        int count = Physics2D.OverlapPoint(worldPos, contactFilter, hits);

        for (int i = 0; i < count; i++)
        {
            IInteractable interactable = hits[i].GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                interactable.Interact();
                return;
            }
        }
    }
}