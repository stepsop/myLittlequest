using UnityEngine;

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
                // Скрываем вариант если предмет уже выдан
                if (option.actionType == ActionType.GiveItem &&
                    option.targetNpcState != null &&
                    option.targetNpcState.itemGiven)
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
        switch (option.actionType)
        {
            case ActionType.GiveItem:
                if (option.giveItem != null && option.targetNpcState != null
                    && !option.targetNpcState.itemGiven)
                {
                    InventoryManager.Instance.AddItem(option.giveItem);
                    option.targetNpcState.itemGiven = true;
                }
                break;

            case ActionType.TakeItem:
                // NPC забирает предмет у игрока
                if (option.takeItem != null)
                    InventoryManager.Instance.RemoveItem(option.takeItem);
                break;

            case ActionType.ExchangeItems:
                // Обмен — забирает takeItem, выдаёт giveItem
                if (option.takeItem != null)
                    InventoryManager.Instance.RemoveItem(option.takeItem);
                if (option.giveItem != null && option.targetNpcState != null
                    && !option.targetNpcState.itemGiven)
                {
                    InventoryManager.Instance.AddItem(option.giveItem);
                    option.targetNpcState.itemGiven = true;
                }
                break;

            case ActionType.SetLoyal:
                // NPC становится лояльным
                if (option.targetNpcState != null)
                    option.targetNpcState.isLoyal = true;
                break;

            case ActionType.LockDialogue:
                // Блокируем диалог навсегда
                if (option.targetNpcState != null)
                    option.targetNpcState.isLocked = true;
                break;

            case ActionType.DestroyObject:
                // Уничтожаем объект, на который ссылается опция
                if (option.objectToDestroy != null)
                    GameObject.Destroy(option.objectToDestroy);
                break;
        }

        if (option.lockDialogue && option.targetNpcState != null)
            option.targetNpcState.isLocked = true;

        // Save() вызывается один раз — не важно какое действие произошло
        SaveManager.Instance?.Save();
    }
}