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
    public ConditionLogic conditionLogic;
    public ItemData requiredItem;
    public NPCDialogue requiredLoyalNpc;

    [Header("Действия при выборе")]
    public DialogueAction[] actions;
}

public enum ConditionLogic
{
    [InspectorName("Есть предмет ИЛИ лоялен")]
    ItemOrLoyal
}