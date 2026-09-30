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
    [SerializeField] private string giverNpcId;
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


    public void Execute()
    {
        switch (type)
        {
            case DialogueActionType.GiveItem:
                if (item == null) break;
                if (string.IsNullOrEmpty(GiverNpcId))
                    Debug.LogWarning("[DialogueAction] GiveItem has no giver NPC ID; repeated rewards cannot be prevented.");
                InventoryManager.Instance.AddItem(item);
                if (!string.IsNullOrEmpty(GiverNpcId))
                {
                    if (SaveManager.Instance != null)
                        SaveManager.Instance.MarkItemGiven(GiverNpcId);
                    else
                        Debug.LogWarning("[DialogueAction] Cannot track the one-time item reward because SaveManager is missing.");
                }
                break;

            case DialogueActionType.SetFlag:
                if (string.IsNullOrEmpty(targetNpcId))
                {
                    Debug.LogWarning("[DialogueAction] SetFlag targetNpcId is empty.");
                    break;
                }

                if (!NPCDialogue.Registry.TryGetValue(targetNpcId, out var npc) || npc == null || npc.State == null)
                {
                    Debug.LogWarning($"[DialogueAction] NPC with ID '{targetNpcId}' was not found or has no state.");
                    break;
                }

                if (!applyLoyal && !applyLocked)
                {
                    Debug.LogWarning($"[DialogueAction] SetFlag for NPC '{targetNpcId}' has no selected flags.");
                    break;
                }

                if (applyLoyal) npc.State.isLoyal = loyalValue;
                if (applyLocked) npc.State.isLocked = lockedValue;
                break;

            case DialogueActionType.DestroyObject:
                if (!string.IsNullOrEmpty(targetNpcId))
                {
                    if (NPCDialogue.Registry.TryGetValue(targetNpcId, out var npcToDestroy) && npcToDestroy != null)
                    {
                        if (SaveManager.Instance != null)
                            SaveManager.Instance.MarkAsDestroyed(targetNpcId);

                        Object.Destroy(npcToDestroy.gameObject);
                    }
                }
                else if (!string.IsNullOrEmpty(targetItemUniqueId))
                {
                    if (PickupItem.Registry.TryGetValue(targetItemUniqueId, out var itemToDestroy) && itemToDestroy != null)
                    {
                        if (SaveManager.Instance != null)
                            SaveManager.Instance.MarkAsDestroyed(targetItemUniqueId);

                        Object.Destroy(itemToDestroy.gameObject);
                    }
                }
                break;
        }
    }
}