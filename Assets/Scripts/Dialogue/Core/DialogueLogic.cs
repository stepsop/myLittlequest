using UnityEngine;

// Вся логика диалогов — отдельно от UI.
// DialogueUI ничего не знает ПОЧЕМУ опция показана или что она делает —
// он просто спрашивает этот класс и рисует результат.
public static class DialogueLogic
{
    // Проверяет — показывать ли эту опцию игроку
public static bool CheckCondition(DialogueData dialogue, DialogueOption option)
    {
        if (option.actions != null)
        {
            foreach (var action in option.actions)
            {
                if (action.type != DialogueActionType.GiveItem) continue;

                if (action.hideOptionAfterGive &&
                    SaveManager.Instance != null &&
                    SaveManager.Instance.HasGivenDialogueReward(dialogue.GetRewardTrackingId(option)))
                    return false;

                if (!string.IsNullOrEmpty(action.GiverNpcId) &&
                    SaveManager.Instance != null &&
                    SaveManager.Instance.HasGivenItem(action.GiverNpcId))
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
    public static void ExecuteAction(DialogueData dialogue, DialogueOption option)
    {
        if (option.actions != null)
        {
            foreach (var action in option.actions)
            {
                if (action.type == DialogueActionType.GiveItem &&
                    action.hideOptionAfterGive &&
                    SaveManager.Instance == null)
                {
                    Debug.LogError("[DialogueLogic] Cannot issue a one-time item reward because SaveManager is missing.");
                    continue;
                }

                bool succeeded = action.Execute();
                if (action.type != DialogueActionType.GiveItem || !succeeded) continue;

                if (action.hideOptionAfterGive)
                    SaveManager.Instance.MarkDialogueRewardGiven(dialogue.GetRewardTrackingId(option));

                if (!string.IsNullOrEmpty(action.GiverNpcId))
                    SaveManager.Instance?.MarkItemGiven(action.GiverNpcId);
            }
        }

        SaveManager.Instance?.Save();
    }
}