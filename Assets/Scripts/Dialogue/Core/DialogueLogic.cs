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

        if (!option.requireItem && !option.requireLoyalty) return true;

        if (option.requireItem &&
            (option.requiredItem == null ||
             InventoryManager.Instance == null ||
             !InventoryManager.Instance.HasItem(option.requiredItem)))
            return false;

        if (option.requireLoyalty &&
            (option.requiredLoyalNpc == null ||
             option.requiredLoyalNpc.State == null ||
             !option.requiredLoyalNpc.State.isLoyal))
            return false;

        return true;
    }

    // Выполняет то, что должно произойти после выбора опции игроком
    public static void ExecuteAction(DialogueData dialogue, DialogueOption option)
    {
        bool optionSucceeded = true;

        if (option.useCondition && option.requireItem && option.consumeRequiredItem)
        {
            if (option.requiredItem == null)
            {
                Debug.LogError("[DialogueLogic] Cannot consume the required item because no item is assigned.");
                return;
            }

            if (InventoryManager.Instance == null)
            {
                Debug.LogError("[DialogueLogic] Cannot consume the required item because InventoryManager is missing.");
                return;
            }

            if (!InventoryManager.Instance.HasItem(option.requiredItem))
            {
                Debug.LogError($"[DialogueLogic] Cannot consume required item '{option.requiredItem.name}' because it is not in the inventory.");
                return;
            }
        }

        if (option.actions != null)
        {
            foreach (var action in option.actions)
            {
                if (action.type == DialogueActionType.GiveItem &&
                    action.hideOptionAfterGive &&
                    SaveManager.Instance == null)
                {
                    Debug.LogError("[DialogueLogic] Cannot issue a one-time item reward because SaveManager is missing.");
                    optionSucceeded = false;
                    continue;
                }

                bool succeeded = action.Execute();
                if (!succeeded)
                {
                    optionSucceeded = false;
                    continue;
                }

                if (action.type == DialogueActionType.GiveItem)
                {
                    if (action.hideOptionAfterGive && SaveManager.Instance != null)
                        SaveManager.Instance.MarkDialogueRewardGiven(dialogue.GetRewardTrackingId(option));

                    if (!string.IsNullOrEmpty(action.GiverNpcId))
                        SaveManager.Instance?.MarkItemGiven(action.GiverNpcId);
                }
            }
        }

        if (option.useCondition && option.requireItem && option.consumeRequiredItem && optionSucceeded)
        {
            InventoryManager.Instance?.RemoveItem(option.requiredItem);
        }

        SaveManager.Instance?.Save();
    }
}