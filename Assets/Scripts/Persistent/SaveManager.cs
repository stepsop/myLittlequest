using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Отвечает за сохранение и загрузку всего состояния игры.
// Использует PlayerPrefs + JSON — просто и без внешних зависимостей.
// Один слот сохранения на всю игру.
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // Ключи в PlayerPrefs — константы чтобы не ошибиться в строках
    private const string SaveExistsKey = "HasSave";
    private const string SaveDataKey = "SaveData";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Проверяем есть ли сохранение — используется в MainMenu
    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveExistsKey);
    }

    // Сохраняем всё состояние игры
    public void Save()
    {
        SaveData data = new SaveData();

        // 1. Текущая сцена
        data.sceneName = SceneManager.GetActiveScene().name;

        // 2. Позиция игрока
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            data.playerX = player.transform.position.x;
            data.playerY = player.transform.position.y;
        }

        // 3. Инвентарь — список предметов и их количество
        foreach (var stack in InventoryManager.Instance.Items)
        {
            data.inventoryItems.Add(new ItemSaveData
            {
                itemName = stack.itemData.name, // Имя SO asset файла
                amount = stack.amount
            });
        }

        // 4. Подобранные предметы из PickupTracker
        data.pickedUpItems = new List<string>(PickupTracker.Instance.GetPickedUpItems());

        // 5. Состояния NPC — берём все NPCState assets
        NPCState[] allStates = Resources.LoadAll<NPCState>("NPCStates");
        foreach (var state in allStates)
        {
            data.npcStates.Add(new NPCStateSaveData
            {
                stateName = state.name,
                isLoyal = state.isLoyal,
                itemGiven = state.itemGiven,
                isLocked = state.isLocked
            });
        }

        // Сериализуем в JSON и сохраняем
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveDataKey, json);
        PlayerPrefs.SetInt(SaveExistsKey, 1);
        PlayerPrefs.Save();

        Debug.Log("Игра сохранена");
    }

    // Загружаем состояние игры.
    // Сцену и позицию игрока грузит SceneLoader — так же, как обычный переход
    // между уровнями (fade out → загрузка → спавн → fade in), только по координатам,
    // а не по SpawnPoint ID.
    public void Load()
    {
        if (!HasSave()) return;

        string json = PlayerPrefs.GetString(SaveDataKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 1. Инвентарь
        InventoryManager.Instance.ClearInventory();
        foreach (var itemData in data.inventoryItems)
        {
            ItemData item = Resources.Load<ItemData>($"Items/{itemData.itemName}");
            if (item != null)
                InventoryManager.Instance.AddItem(item, itemData.amount);
        }

        // 2. Подобранные предметы
        PickupTracker.Instance.LoadPickedUpItems(data.pickedUpItems);

        // 3. Состояния NPC
        NPCState[] allStates = Resources.LoadAll<NPCState>("NPCStates");
        foreach (var saved in data.npcStates)
        {
            foreach (var state in allStates)
            {
                if (state.name != saved.stateName) continue;
                state.isLoyal = saved.isLoyal;
                state.itemGiven = saved.itemGiven;
                state.isLocked = saved.isLocked;
                break;
            }
        }

        // 4. Сцена + позиция игрока — через SceneLoader, с fade-эффектом,
        // как обычный переход. IsTransitioning и сброс UI-состояний
        // SceneLoader делает сам.
        Vector3 targetPosition = new Vector3(data.playerX, data.playerY, 0);

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneAtPosition(data.sceneName, targetPosition);
        }
        else
        {
            Debug.LogError("SaveManager: SceneLoader.Instance == null. Загрузка сохранения невозможна без SceneLoader в GameManager prefab.");
        }

        Debug.Log($"Загрузка: сцена {data.sceneName}, позиция ({data.playerX}, {data.playerY})");
    }

    // Удаляем сохранение — при новой игре
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveExistsKey);
        PlayerPrefs.DeleteKey(SaveDataKey);
        PlayerPrefs.Save();
    }

    // Контейнер всех данных для сохранения
    // [System.Serializable] нужен чтобы JsonUtility мог сериализовать класс
    [System.Serializable]
    public class SaveData
    {
        public string sceneName;
        public float playerX;
        public float playerY;
        public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();
        public List<string> pickedUpItems = new List<string>();
        public List<NPCStateSaveData> npcStates = new List<NPCStateSaveData>();
    }

    [System.Serializable]
    public class ItemSaveData
    {
        public string itemName; // Имя SO asset файла
        public int amount;
    }

    [System.Serializable]
    public class NPCStateSaveData
    {
        public string stateName; // Имя NPCState SO asset файла
        public bool isLoyal;
        public bool itemGiven;
        public bool isLocked;
    }
}