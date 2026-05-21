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
        if (!hintObject.activeSelf) return;

        // Подпись всегда «выпрямлена» к камере
        hintObject.transform.position = transform.position + Vector3.up * heightOffset + offset;
        hintObject.transform.rotation = mainCamera.transform.rotation;
    }

    #region IHoverable
    public void OnMouseEnter()
    {
        hintObject?.SetActive(true);
    }

    public void OnMouseExit()
    {
        hintObject?.SetActive(false);
    }
    #endregion

    // Для ручного вызова из других скриптов (не обязательно)
    public void Show() => hintObject?.SetActive(true);
    public void Hide() => hintObject?.SetActive(false);

    private void OnDestroy()
    {
        if (hintObject != null)
            Destroy(hintObject);
    }
}
