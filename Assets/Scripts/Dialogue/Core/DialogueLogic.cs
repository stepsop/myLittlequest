// Вся логика диалогов — отдельно от UI.
// DialogueUI ничего не знает ПОЧЕМУ опция показана или что она делает —
// он просто спрашивает этот класс и рисует результат.
public static class DialogueLogic
{
    // Проверяет — показывать ли эту опцию игроку
    public static bool CheckCondition(DialogueOption option)
    {
        if (option.actions != null)
        {
            foreach (var action in option.actions)
            {
                if (action.type == DialogueActionType.GiveItem &&
                    action.giverNpc != null && action.giverNpc.State != null &&
                    action.giverNpc.State.itemGiven)
                    return false;
            }
        }

        if (!option.useCondition) return true;

        bool hasItem = option.requiredItem != null &&
                       InventoryManager.Instance.HasItem(option.requiredItem);
        bool isLoyal = option.requiredLoyalNpc != null &&
                       option.requiredLoyalNpc.State != null &&
                       option.requiredLoyalNpc.State.isLoyal;

        return option.conditionLogic == ConditionLogic.ItemOrLoyal
            ? hasItem || isLoyal
            : hasItem && isLoyal;
    }

    // Выполняет то, что должно произойти после выбора опции игроком
    public static void ExecuteAction(DialogueOption option)
    {
        if (option.actions != null)
        {
            foreach (var action in option.actions)
                action.Execute();
        }

        SaveManager.Instance?.Save();
    }
}