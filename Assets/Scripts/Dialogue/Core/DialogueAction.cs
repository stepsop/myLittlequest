using UnityEngine;

public enum DialogueActionType
{
    GiveItem,
    SetFlag,
    DestroyObject
}

[System.Serializable]
public class DialogueAction
{
    public DialogueActionType type;

    [Header("GiveItem")]
    public ItemData item;
    public GameObject itemPrefab;
    [InspectorName("Скрыть вариант после выдачи")]
    public bool hideOptionAfterGive = true;
    [SerializeField, HideInInspector] private string giverNpcId;
    [HideInInspector] public NPCDialogue giverNpc;
    public string GiverNpcId => !string.IsNullOrEmpty(giverNpcId) ? giverNpcId : giverNpc != null ? giverNpc.NpcID : string.Empty;

    [Header("SetFlag")]
    public bool loyalValue;
    public bool lockedValue;
    public bool applyLoyal;
    public bool applyLocked;

    [Header("Identifiers")]
    [SerializeField] private string targetNpcId;
    [SerializeField] private string targetItemUniqueId;


    public bool Execute()
    {
        switch (type)
        {
            case DialogueActionType.GiveItem:
                ItemData itemToGive = item;
                if (itemPrefab != null)
                {
                    PickupItem pickupItem = itemPrefab.GetComponent<PickupItem>();
                    if (pickupItem == null)
                    {
                        Debug.LogError($"[DialogueAction] Prefab '{itemPrefab.name}' has no PickupItem component.", itemPrefab);
                        return false;
                    }

                    itemToGive = pickupItem.Data;
                    if (itemToGive == null)
                    {
                        Debug.LogError($"[DialogueAction] PickupItem prefab '{itemPrefab.name}' has no ItemData assigned.", itemPrefab);
                        return false;
                    }
                }

                if (itemToGive == null) return false;
                if (InventoryManager.Instance == null)
                {
                    Debug.LogError("[DialogueAction] Cannot give an item because InventoryManager is missing.");
                    return false;
                }

                InventoryManager.Instance.AddItem(itemToGive);
                return true;

            case DialogueActionType.SetFlag:
                if (string.IsNullOrEmpty(targetNpcId))
                {
                    Debug.LogWarning("[DialogueAction] SetFlag targetNpcId is empty.");
                    return false;
                }

                if (!NPCDialogue.Registry.TryGetValue(targetNpcId, out var npc) || npc == null || npc.State == null)
                {
                    Debug.LogWarning($"[DialogueAction] NPC with ID '{targetNpcId}' was not found or has no state.");
                    return false;
                }

                if (!applyLoyal && !applyLocked)
                {
                    Debug.LogWarning($"[DialogueAction] SetFlag for NPC '{targetNpcId}' has no selected flags.");
                    return false;
                }

                if (applyLoyal) npc.State.isLoyal = loyalValue;
                if (applyLocked) npc.State.isLocked = lockedValue;
                return true;

            case DialogueActionType.DestroyObject:
                if (!string.IsNullOrEmpty(targetNpcId))
                {
                    if (!NPCDialogue.Registry.TryGetValue(targetNpcId, out var npcToDestroy) || npcToDestroy == null)
                        return false;

                    if (SaveManager.Instance != null)
                        SaveManager.Instance.MarkAsDestroyed(targetNpcId);

                    Object.Destroy(npcToDestroy.gameObject);
                    return true;
                }

                if (!string.IsNullOrEmpty(targetItemUniqueId))
                {
                    if (!PickupItem.Registry.TryGetValue(targetItemUniqueId, out var itemToDestroy) || itemToDestroy == null)
                        return false;

                    if (SaveManager.Instance != null)
                        SaveManager.Instance.MarkAsDestroyed(targetItemUniqueId);

                    Object.Destroy(itemToDestroy.gameObject);
                    return true;
                }

                return false;
        }

        return false;
    }
}