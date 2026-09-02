using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Give Item")]
public class GiveItemAction : ScriptableObject, IDialogueAction
{
    [SerializeField] private ItemData item;

    public void Execute()
    {
        if (item == null) return;
        InventoryManager.Instance.AddItem(item);
    }
}