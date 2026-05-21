using UnityEngine;

[CreateAssetMenu(menuName = "Диалог/Состояние НПС")]
public class NPCState : ScriptableObject
{
    [Header("Состояние НПС")]
    [Tooltip("НПС лоялен к игроку")]
    public bool isLoyal;

    [Tooltip("Предмет уже выдан — повторно не выдаётся")]
    public bool itemGiven;

    [Tooltip("Диалог заблокирован навсегда")]
    public bool isLocked;

    // Вызывается автоматически когда NPCDialogue стартует
    private void OnEnable()
    {
        // OnEnable вызывается при загрузке SO — сбрасываем состояние
        // Это происходит при каждом запуске игры в Editor и в билде
        Reset();
    }

    public void Reset()
    {
        isLoyal = false;
        itemGiven = false;
        isLocked = false;
    }
}