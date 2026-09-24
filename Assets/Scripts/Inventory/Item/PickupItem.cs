using System.Collections.Generic;
using UnityEngine;

// Универсальный скрипт для любого подбираемого предмета на сцене.
// Чтобы добавить новый предмет — создай prefab из этого скрипта,
// измени название объекта на сцене и назначь нужный ItemData SO.
public class PickupItem : MonoBehaviour, IInteractable
{
    public static readonly Dictionary<string, PickupItem> Registry = new();

    [Header("Данные предмета — назначь SO из Assets/Inventory/Items/")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private string uniqueId;

    private string itemID;

    public string ItemName => itemData?.itemName;

    private void Start()
    {
        // 1. Проверяем, был ли предмет принудительно удален через DialogueAction
        if (!string.IsNullOrEmpty(uniqueId) && SaveManager.Instance != null && SaveManager.Instance.IsDestroyed(uniqueId))
        {
            Destroy(gameObject);
            return;
        }

        itemID = gameObject.scene.name + "_" +
                (!string.IsNullOrEmpty(uniqueId) ? uniqueId : gameObject.name);

        if (!string.IsNullOrEmpty(uniqueId)) 
            Registry[uniqueId] = this;

        // 2. Проверяем, был ли предмет подобран игроком штатно
        if (PickupTracker.Instance != null && PickupTracker.Instance.IsPickedUp(itemID))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(uniqueId)) 
            Registry.Remove(uniqueId);
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

        if (PickupTracker.Instance != null && PickupTracker.Instance.IsPickedUp(itemID))
            return;

        PickupTracker.Instance?.MarkPickedUp(itemID);
        InventoryManager.Instance.AddItem(itemData);

        Destroy(gameObject);
    }
}