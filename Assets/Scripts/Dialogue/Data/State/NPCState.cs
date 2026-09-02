// Assets/Scripts/NPC/NPCState.cs
// Оставь только это:

using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC State")]
public class NPCState : ScriptableObject
{
    [Header("Состояние НПС")]
    public bool isLoyal;
    public bool itemGiven;
    public bool isLocked;

    public bool isDestroyed;

    // Сброс используется только при старте новой игры
    public void Reset()
    {
        isLoyal = false;
        itemGiven = false;
        isLocked = false;
        isDestroyed = false;
    }

    public NPCState CreateInstance()
    {
        var instance = Instantiate(this);
        instance.name = name; // имя нужно для SaveManager (поиск по name)
        return instance;
    }
}