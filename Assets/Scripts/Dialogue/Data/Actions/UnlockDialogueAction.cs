using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Unlock Dialogue")]
public class UnlockDialogueAction : ScriptableObject, IDialogueAction
{
    [SerializeField] private NPCState targetState;

    public void Execute()
    {
        if (targetState != null) targetState.isLocked = false;
    }
}