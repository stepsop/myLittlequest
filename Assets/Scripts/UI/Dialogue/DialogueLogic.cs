// Вся логика диалогов — отдельно от UI.
// DialogueUI ничего не знает ПОЧЕМУ опция показана или что она делает —
// он просто спрашивает этот класс и рисует результат.
public static class DialogueLogic
{
    // Проверяет — показывать ли эту опцию игроку
    public static bool CheckCondition(DialogueOption option)
    {
        switch (option.conditionType)
        {
            case ConditionType.None:
                if (option.hideIfItemGiven != null && option.hideIfItemGiven.itemGiven)
                    return false;
                return true;

            case ConditionType.HasItem:
                return option.requiredItem != null &&
                       InventoryManager.Instance.HasItem(option.requiredItem);

            case ConditionType.NPCIsLoyal:
                return option.npcState != null && option.npcState.isLoyal;

            case ConditionType.NPCNotLoyal:
                return option.npcState != null && !option.npcState.isLoyal;

            default:
                return true;
        }
    }

    // Выполняет то, что должно произойти после выбора опции игроком
    public static void ExecuteAction(DialogueOption option)
    {
        if (option.actions != null)
        {
            foreach (var so in option.actions)
                (so as IDialogueAction)?.Execute();
        }

        SaveManager.Instance?.Save();
    }
}