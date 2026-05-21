using UnityEngine;

// Универсальный скрипт для любого подбираемого предмета на сцене.
// Чтобы добавить новый предмет — создай prefab из этого скрипта,
// измени название объекта на сцене и назначь нужный ItemData SO.
public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Данные предмета — назначь SO из Assets/Inventory/Items/")]
    public ItemData itemData;

    private bool playerInside;
    private string itemID;

    private void Start()
    {
        // ID = сцена + имя объекта — уникален пока объект не переименован
        itemID = gameObject.scene.name + "_" + gameObject.name;

        // Если уже подбирали — скрываем сразу при загрузке сцены
        if (PickupTracker.Instance != null && PickupTracker.Instance.IsPickedUp(itemID))
            gameObject.SetActive(false);
    }

    public bool CanInteract() => playerInside;

    public void Interact()
    {
        PickupTracker.Instance?.MarkPickedUp(itemID);
        InventoryManager.Instance.AddItem(itemData);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }
}