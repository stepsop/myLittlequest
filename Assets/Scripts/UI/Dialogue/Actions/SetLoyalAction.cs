using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Set Loyal")]
public class SetLoyalAction : ScriptableObject, IDialogueAction
{
    [SerializeField] private NPCState targetState;

    public void Execute()
    {
        if (targetState != null) targetState.isLoyal = true;
    }
}