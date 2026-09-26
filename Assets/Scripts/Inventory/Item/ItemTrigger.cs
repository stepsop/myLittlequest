using UnityEngine;

// Универсальный триггер — требует предмет из инвентаря, спавнит reward объект.
// Чтобы сделать новый триггер — создай prefab, назначь requiredItem и rewardPrefab.
public class ItemTrigger : MonoBehaviour, IInteractable
{
    [Header("Что нужно иметь выбранным в инвентаре")]
    public ItemData requiredItem;

    [Header("Объект который появится после использования")]
    public GameObject rewardPrefab; // Перетащи prefab PickupItem с нужным ItemData

    [Header("Точка появления награды")]
    public Transform spawnPoint;

    [Header("Фразы если не тот предмет")]
    public FailPhraseDatabase wrongItemPhrases;

    [SerializeField] private SpeechBubble playerSpeechBubble;

    private bool playerInside;

    // Однократное использование — после активации триггер больше не работает
    private bool alreadyUsed;

    public bool CanInteract() => playerInside && !alreadyUsed;

    public void Interact()
    {
        if (!CanInteract()) return;

        if (InventoryManager.Instance.SelectedItem != requiredItem)
        {
            var phrase = wrongItemPhrases != null ? wrongItemPhrases.GetRandom() : null;
            if (phrase != null)
                playerSpeechBubble?.Show(phrase.text, phrase.audio);
            return;
        }

        if (rewardPrefab != null && spawnPoint != null)
        {
            GameObject reward = Instantiate(rewardPrefab, spawnPoint.position, Quaternion.identity);
            reward.name = rewardPrefab.name;
        }

        InventoryManager.Instance.ConsumeSelectedItem();
        alreadyUsed = true;
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