using UnityEngine;
using TMPro;

// Один общий hint на всю сцену.
// Объекты запрашивают его через Show/Hide — не создают свои копии.
public class InteractionHintUI : MonoBehaviour
{
    public static InteractionHintUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI label;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        // Всегда смотрит лицом к камере (важно для World Space canvas)
        if (mainCamera != null)
            transform.rotation = mainCamera.transform.rotation;
    }

    // Показать подсказку над указанной позицией
    public void Show(string text, Vector3 worldPosition, float heightOffset = 1f)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        label.text = text;
        transform.position = worldPosition + Vector3.up * heightOffset;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}