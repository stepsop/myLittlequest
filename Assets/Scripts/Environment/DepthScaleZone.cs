using UnityEngine;

public enum DepthScaleMode
{
    ShrinkOnApproach,
    GrowOnApproach
}

// Требует Collider2D (IsTrigger). Пока игрок внутри — масштаб пропорционален расстоянию до этого объекта.
public class DepthScaleZone : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private DepthScaleMode mode = DepthScaleMode.ShrinkOnApproach;

    [Header("Distance")]
    [SerializeField] private float startDistance = 10f;
    [SerializeField] private float endDistance = 1f;

    [Header("Scale")]
    [SerializeField, Range(0.01f, 3f)] private float extremeScale = 0.3f;

    private Transform playerVisual;
    private Vector3 baseScale;
    private bool playerInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerVisual visual = other.GetComponentInChildren<PlayerVisual>();
        if (visual == null) return;

        playerVisual = visual.transform;
        baseScale = playerVisual.localScale;
        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        if (playerVisual != null)
            playerVisual.localScale = baseScale;
    }

    private void Update()
    {
        if (!playerInside || playerVisual == null) return;

        float distance = Vector2.Distance(transform.position, playerVisual.position);
        float t = Mathf.InverseLerp(startDistance, endDistance, distance);

        float scaleFactor = mode == DepthScaleMode.ShrinkOnApproach
            ? Mathf.Lerp(1f, extremeScale, t)
            : Mathf.Lerp(1f, extremeScale, t);

        playerVisual.localScale = baseScale * scaleFactor;
    }
}