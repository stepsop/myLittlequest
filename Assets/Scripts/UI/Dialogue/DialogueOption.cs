using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    [Header("Текст кнопки")]
    public string text;

    [Header("Следующий диалог (null = закрыть)")]
    public DialogueData nextDialogue;

    [Header("Условие показа")]
    public ConditionType conditionType;
    public ItemData requiredItem;
    public NPCState npcState;

    [Header("Действие при выборе")]
    public ActionType actionType;
    public ItemData giveItem;
    public ItemData takeItem;
    public NPCState targetNpcState;
    public bool lockDialogue;

    [Header("Фраза НПС после блокировки")]
    public string lockedPhrase;
    public AudioClip lockedAudio;

    [InspectorName("Уничтожить объект")]
    public GameObject objectToDestroy;
}

public enum ConditionType
{
    [InspectorName("Всегда показывать")]
    None,
    [InspectorName("Есть предмет в инвентаре")]
    HasItem,
    [InspectorName("NPC лоялен")]
    NPCIsLoyal,
    [InspectorName("NPC не лоялен")]
    NPCNotLoyal
}

public enum ActionType
{
    [InspectorName("Нет действия")]
    None,
    [InspectorName("NPC выдаёт предмет")]
    GiveItem,
    [InspectorName("NPC забирает предмет")]
    TakeItem,
    [InspectorName("Обмен предметами")]
    ExchangeItems,
    [InspectorName("NPC становится лояльным")]
    SetLoyal,
    [InspectorName("Заблокировать диалог")]
    LockDialogue,
    [InspectorName("Уничтожить объект")]
    DestroyObject
}