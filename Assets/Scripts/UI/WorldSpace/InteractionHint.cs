using UnityEngine;
using TMPro;


public class InteractionHint : MonoBehaviour, IHoverable
{
    [Header("Префаб подсказки")]
    [SerializeField] private GameObject hintPrefab;

    [Header("Настройки")]
    [SerializeField] private float heightOffset = 1f;
    [SerializeField] private Vector3 offset = Vector3.zero; // если нужно смещение

    private GameObject hintObject;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // Создаём подсказку (но не активируем)
        hintObject = Instantiate(hintPrefab);
        TMP_Text label = hintObject.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            PickupItem pickup = GetComponent<PickupItem>();
            label.text = (pickup?.itemData?.itemName) ?? gameObject.name;
        }

        hintObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (hintObject == null || !hintObject.activeSelf) return;
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (mainCamera == null) return;

        // Подпись всегда «выпрямлена» к камере
        hintObject.transform.position = transform.position + Vector3.up * heightOffset + offset;
        hintObject.transform.rotation = mainCamera.transform.rotation;
    }

    #region IHoverable
    public void OnMouseEnter()
    {
        if (hintObject != null)
            hintObject.SetActive(true);
    }

    public void OnMouseExit()
    {
        if (hintObject != null)
            hintObject.SetActive(false);
    }
    #endregion

    // Для ручного вызова из других скриптов (не обязательно)
    public void Show()
    {
        if (hintObject != null)
            hintObject.SetActive(true);
    }

    public void Hide()
    {
        if (hintObject != null)
            hintObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (hintObject != null)
            Destroy(hintObject);
    }
}
