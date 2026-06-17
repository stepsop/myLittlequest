// Assets/Scripts/NPC/NPCState.cs
// Оставь только это:

using UnityEngine;

[CreateAssetMenu(menuName = "Диалог/Состояние НПС")]
public class NPCState : ScriptableObject
{
    [Header("Состояние НПС")]
    public bool isLoyal;
    public bool itemGiven;
    public bool isLocked;

    // Сброс используется только при старте новой игры
    public void Reset()
    {
        isLoyal   = false;
        itemGiven = false;
        isLocked  = false;
    }
}