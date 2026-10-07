using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    [Header("Текст кнопки")]
    public string text;

    [Header("Следующий диалог (null = закрыть)")]
    public DialogueData nextDialogue;

    [Header("Условие показа")]
    public bool useCondition;
    [HideInInspector] public ConditionLogic conditionLogic;
    [InspectorName("Есть предмет")]
    public bool requireItem;
    [InspectorName("Лоялен")]
    public bool requireLoyalty;
    [InspectorName("Предмет")]
    public ItemData requiredItem;
    [InspectorName("Лояльный NPC")]
    public NPCDialogue requiredLoyalNpc;

    [Header("Действия при выборе")]
    public DialogueAction[] actions;

    [HideInInspector]
    public string rewardTrackingId = System.Guid.NewGuid().ToString("N");
}

public enum ConditionLogic
{
    [InspectorName("Есть предмет ИЛИ лоялен")]
    ItemOrLoyal
}