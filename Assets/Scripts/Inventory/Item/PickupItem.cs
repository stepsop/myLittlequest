using UnityEngine;

// Универсальный скрипт для любого подбираемого предмета на сцене.
// Чтобы добавить новый предмет — создай prefab из этого скрипта,
// измени название объекта на сцене и назначь нужный ItemData SO.
public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Данные предмета — назначь SO из Assets/Inventory/Items/")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private string uniqueId; // задать вручную в инспекторе, уникально в пределах сцены

    private string itemID;

    public string ItemName => itemData?.itemName;

    private void Start()
    {
        itemID = gameObject.scene.name + "_" +
                (!string.IsNullOrEmpty(uniqueId) ? uniqueId : gameObject.name);

        if (PickupTracker.Instance != null &&
            PickupTracker.Instance.IsPickedUp(itemID))
        {
            gameObject.SetActive(false);
        }
    }

    public bool CanInteract()
    {
        Transform player = PlayerMovement.Instance?.transform;
        if (player == null) return false;
        return Vector2.Distance(player.position, transform.position) <= interactDistance;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        if (PickupTracker.Instance != null &&
            PickupTracker.Instance.IsPickedUp(itemID))
            return;

        PickupTracker.Instance?.MarkPickedUp(itemID);
        InventoryManager.Instance.AddItem(itemData);
        SpeechBubble.Instance?.Show(itemData.pickupPhrase);

        Destroy(gameObject);
    }
}