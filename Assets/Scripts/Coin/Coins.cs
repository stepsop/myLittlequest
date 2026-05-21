using UnityEngine;

public class Coin : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData coinItemData;
    [SerializeField] private int coinAmount = 1;

    private bool playerInside;
    private string itemID;

    private void Start()
    {
        itemID = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                 + "_" + gameObject.name;

        if (PickupTracker.Instance != null && PickupTracker.Instance.IsPickedUp(itemID))
            gameObject.SetActive(false);
    }

    public bool CanInteract() => playerInside;

    public void Interact()
    {
        if (!CanInteract()) return;

        PickupTracker.Instance?.MarkPickedUp(itemID);
        InventoryManager.Instance.AddItem(coinItemData, coinAmount);
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