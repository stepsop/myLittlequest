using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<ItemStack> items = new();
    public List<ItemStack> Items => items;

    public ItemData SelectedItem { get; private set; }

    [System.Serializable]
    public class ItemStack
    {
        public ItemData itemData;
        public int amount;

        public ItemStack(ItemData data, int qty)
        {
            itemData = data;
            amount = qty;
        }
    }

    private void Awake()
    {
        // Единственный инвентарь на всю игру (между сценами).
        // Если где-то в сцене случайно лежит ещё один InventoryManager —
        // он не должен перезаписать Instance и "обнулить" список предметов.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Иначе при переходе сцены список предметов сбрасывается.
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        ItemStack existingStack = items.Find(stack => stack.itemData == item);

        if (existingStack != null)
            existingStack.amount += amount;
        else
            items.Add(new ItemStack(item, amount));

        InventoryUI.Instance.RefreshUI(items);
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        ItemStack stack = items.Find(s => s.itemData == item);
        if (stack == null) return;

        stack.amount -= amount;
        if (stack.amount <= 0)
            items.Remove(stack);

        InventoryUI.Instance.RefreshUI(items);
    }

    public bool HasItem(ItemData item)
    {
        return items.Exists(stack => stack.itemData == item);
    }

    public int GetItemCount(ItemData item)
    {
        ItemStack stack = items.Find(s => s.itemData == item);
        return stack?.amount ?? 0;
    }

    public void SelectItem(ItemData item)
    {
        if (SelectedItem == item)
        {
            SelectedItem = null;
            InventoryUI.Instance.ClearSelection();
            return;
        }

        SelectedItem = item;
        InventoryUI.Instance.Highlight(item);
    }

    public bool SpendCoins(ItemData coinItem, int amount)
    {
        ItemStack coinStack = items.Find(s => s.itemData == coinItem);

        if (coinStack == null || coinStack.amount < amount)
            return false;

        coinStack.amount -= amount;
        if (coinStack.amount <= 0)
            items.Remove(coinStack);

        InventoryUI.Instance.RefreshUI(items);
        return true;
    }

    public void ConsumeSelectedItem()
    {
        if (SelectedItem == null) return;

        if (SelectedItem.consumable)
        {
            RemoveItem(SelectedItem);
            SelectedItem = null;
            InventoryUI.Instance.RefreshUI(items);
        }
    }
    
    // Полная очистка инвентаря — используется при загрузке сохранения
    public void ClearInventory()
    {
        items.Clear();
        SelectedItem = null;
        InventoryUI.Instance?.RefreshUI(items);
    }
}