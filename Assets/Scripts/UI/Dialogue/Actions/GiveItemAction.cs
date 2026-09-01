using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Give Item")]
public class GiveItemAction : ScriptableObject, IDialogueAction
{
    [SerializeField] private ItemData item;
    [SerializeField] private NPCState targetState;

    public void Execute()
    {
        if (item == null || targetState == null || targetState.itemGiven) return;
        InventoryManager.Instance.AddItem(item);
        targetState.itemGiven = true;
    }
}