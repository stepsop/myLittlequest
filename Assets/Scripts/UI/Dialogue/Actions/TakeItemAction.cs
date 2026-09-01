using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Take Item")]
public class TakeItemAction : ScriptableObject, IDialogueAction
{
    [SerializeField] private ItemData item;

    public void Execute()
    {
        if (item != null) InventoryManager.Instance.RemoveItem(item);
    }
}