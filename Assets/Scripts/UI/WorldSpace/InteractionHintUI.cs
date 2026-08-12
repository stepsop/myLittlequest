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

    /// <summary>
    /// Показать подсказку в позиции указанного Transform (Hint).
    /// </summary>
    public void Show(string text, Transform hintPoint)
    {
        if (label == null)
        {
            Debug.LogError("InteractionHintUI: label не назначен.", this);
            return;
        }

        if (hintPoint == null)
        {
            Debug.LogError("InteractionHintUI: hintPoint равен null.", this);
            return;
        }

        if (mainCamera == null)
            mainCamera = Camera.main;

        label.text = text;
        transform.position = hintPoint.position;
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

        UIActivator.ActivateHierarchy(hintUI.transform);
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
}