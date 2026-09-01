using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Lock Dialogue")]
public class LockDialogueAction : ScriptableObject, IDialogueAction
{
    [SerializeField] private NPCState targetState;

    public void Execute()
    {
        if (targetState != null) targetState.isLocked = true;
    }
}