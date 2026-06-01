using System.Collections.Generic;
using UnityEngine;


public class PickupTracker : MonoBehaviour
{
    public static PickupTracker Instance { get; private set; }

    // Список подобранных предметов
    // Ключ — уникальный ID предмета (сцена + имя объекта)
    private HashSet<string> pickedUpItems = new HashSet<string>();

    private void Awake()
    {
        // Этот менеджер должен быть единственным на всю игру.
        // Если сцена содержит PickupTracker, а GameRoot уже создал глобальный —
        // то "сценовый" экземпляр уничтожится и не затрёт данные.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Нужен, чтобы HashSet не сбрасывался при переходах сцен.
        //DontDestroyOnLoad(gameObject);
    }

    // Запомнить что предмет подобран
    public void MarkPickedUp(string itemID)
    {
        pickedUpItems.Add(itemID);
    }

    // Проверить подобран ли предмет
    public bool IsPickedUp(string itemID)
    {
        return pickedUpItems.Contains(itemID);
    }
    // Возвращаем копию HashSet для сохранения
    public HashSet<string> GetPickedUpItems()
    {
        return new HashSet<string>(pickedUpItems);
    }

    // Загружаем список подобранных предметов из сохранения
    public void LoadPickedUpItems(List<string> items)
    {
        pickedUpItems.Clear();
        foreach (var item in items)
            pickedUpItems.Add(item);
    }
}