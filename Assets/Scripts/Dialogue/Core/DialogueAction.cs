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

    [Header("DestroyObject")]
    public GameObject targetObject;

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
                if (targetObject != null) Object.Destroy(targetObject);
                break;
        }
    }
}