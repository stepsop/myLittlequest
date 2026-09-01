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

    [Header("Скрыть если предмет уже выдан")]
    public NPCState hideIfItemGiven;

    [Header("Действия при выборе")]
    public ScriptableObject[] actions; // каждый реализует IDialogueAction

    public bool lockDialogue;

    [Header("Фраза НПС после блокировки")]
    public string lockedPhrase;
    public AudioClip lockedAudio;
    
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