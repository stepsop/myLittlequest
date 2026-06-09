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
        RegisterInstance();
        Hide();
    }

    private void OnEnable()
    {
        RegisterInstance();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void LateUpdate()
    {
        // Всегда смотрит лицом к камере (важно для World Space canvas)
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
            transform.rotation = mainCamera.transform.rotation;
    }

    // Показать подсказку над указанной позицией
    public void Show(string text, Vector3 worldPosition, float heightOffset = 1f)
    {
        if (label == null)
        {
            Debug.LogError("InteractionHintUI: label не назначен.", this);
            return;
        }

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

    public static InteractionHintUI GetOrFindInstance()
    {
        if (Instance != null)
            return Instance;

        InteractionHintUI hintUI = FindAnyObjectByType<InteractionHintUI>(FindObjectsInactive.Include);
        if (hintUI == null)
            return null;

        hintUI.ActivateHierarchy();
        hintUI.RegisterInstance();
        hintUI.Hide();

        return Instance;
    }

    private void RegisterInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void ActivateHierarchy()
    {
        Transform current = transform;
        while (current != null)
        {
            if (!current.gameObject.activeSelf)
                current.gameObject.SetActive(true);

            current = current.parent;
        }
    }
}
