using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель")]
    [SerializeField] private Transform target;

    [Header("Настройки")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Camera cam;
    private float minX, maxX, minY, maxY;
    private bool hasBounds = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        // Ищем границы автоматически при старте сцены
        FindBounds();
    }

    // Публичный метод — вызывай его при переходе на новый уровень
    // чтобы камера нашла новый Floor
    public void FindBounds()
    {
        // Ищем объект с тегом LevelBounds в текущей сцене
        GameObject boundsObject = GameObject.FindWithTag("LevelBounds");

        if (boundsObject == null)
        {
            Debug.LogWarning("CameraFollow: объект с тегом LevelBounds не найден!");
            hasBounds = false;
            return;
        }

        Collider2D boundsCollider = boundsObject.GetComponent<Collider2D>();

        if (boundsCollider == null)
        {
            Debug.LogWarning("CameraFollow: на LevelBounds нет Collider2D!");
            hasBounds = false;
            return;
        }

        // Берём границы из коллайдера
        minX = boundsCollider.bounds.min.x;
        maxX = boundsCollider.bounds.max.x;
        minY = boundsCollider.bounds.min.y;
        maxY = boundsCollider.bounds.max.y;

        hasBounds = true;
        Debug.Log("CameraFollow: границы найдены!");
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        // Применяем границы только если они найдены
        if (hasBounds)
        {
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = cam.orthographicSize * cam.aspect;

            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                minX + camHalfWidth,
                maxX - camHalfWidth
            );

            targetPosition.y = Mathf.Clamp(
                targetPosition.y,
                minY + camHalfHeight,
                maxY - camHalfHeight
            );
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    private void OnDrawGizmos()
    {
        if (!hasBounds) return;

        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}