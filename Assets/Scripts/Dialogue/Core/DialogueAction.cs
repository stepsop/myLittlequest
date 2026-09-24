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
    public NPCDialogue giverNpc;

    [Header("SetFlag")]
    public NPCDialogue targetNpc;
    public bool loyalValue;
    public bool lockedValue;
    public bool applyLoyal;
    public bool applyLocked;

    [Header("DestroyObject — заполни ОДНО из полей")]
    [SerializeField] private string targetNpcId;
    [SerializeField] private string targetItemUniqueId;

    public void Execute()
    {
        switch (type)
        {
            case DialogueActionType.GiveItem:
                if (item == null) break;
                InventoryManager.Instance.AddItem(item);
                if (giverNpc != null && giverNpc.State != null)
                    giverNpc.State.itemGiven = true;
                break;

            case DialogueActionType.SetFlag:
                if (targetNpc == null || targetNpc.State == null) break;
                if (applyLoyal) targetNpc.State.isLoyal = loyalValue;
                if (applyLocked) targetNpc.State.isLocked = lockedValue;
                break;

            case DialogueActionType.DestroyObject:
                if (!string.IsNullOrEmpty(targetNpcId))
                {
                    if (NPCDialogue.Registry.TryGetValue(targetNpcId, out var npc) && npc != null)
                    {
                        if (SaveManager.Instance != null)
                            SaveManager.Instance.MarkAsDestroyed(targetNpcId);

                        Object.Destroy(npc.gameObject);
                    }
                }
                else if (!string.IsNullOrEmpty(targetItemUniqueId))
                {
                    if (PickupItem.Registry.TryGetValue(targetItemUniqueId, out var item) && item != null)
                    {
                        if (SaveManager.Instance != null)
                            SaveManager.Instance.MarkAsDestroyed(targetItemUniqueId);

                        Object.Destroy(item.gameObject);
                    }
                }
                break;
        }
    }
}