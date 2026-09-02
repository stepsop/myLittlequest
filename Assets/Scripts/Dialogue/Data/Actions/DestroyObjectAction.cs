using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Destroy Object")]
public class DestroyObjectAction : ScriptableObject, IDialogueAction
{
    [SerializeField] private GameObject target;

    public void Execute()
    {
        if (target != null) Object.Destroy(target);
    }
}