using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveExistsKey = "HasSave";
    private const string SaveDataKey = "SaveData";

    // Хранилище уничтоженных объектов (NPC, квестовые предметы и др.)
    private HashSet<string> destroyedObjectIds = new HashSet<string>();

    // Кэш состояний NPC со всех посещенных сцен в текущей сессии
    private Dictionary<string, NPCStateSaveData> cachedNpcStates = new Dictionary<string, NPCStateSaveData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Подписываемся на смену/загрузку сцен
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- МЕТОДЫ ДЛЯ ТРЕКИНГА УНИЧТОЖЕНИЯ ---

    /// <summary>
    /// Помечает объект как уничтоженный (запоминает его ID).
    /// </summary>
    public void MarkAsDestroyed(string id)
    {
        if (!string.IsNullOrEmpty(id))
            destroyedObjectIds.Add(id);
    }

    /// <summary>
    /// Проверяет, был ли объект с таким ID уничтожен ранее.
    /// </summary>
    public bool IsDestroyed(string id)
    {
        return !string.IsNullOrEmpty(id) && destroyedObjectIds.Contains(id);
    }

    // --- СОХРАНЕНИЕ И ЗАГРУЗКА ---

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveExistsKey);
    }

    public void Save()
    {
        SaveData data = new SaveData();

        // 1. Сцена и позиция игрока
        data.sceneName = SceneManager.GetActiveScene().name;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            data.playerX = player.transform.position.x;
            data.playerY = player.transform.position.y;
        }

        // 2. Инвентарь
        if (InventoryManager.Instance != null)
        {
            foreach (var stack in InventoryManager.Instance.Items)
            {
                data.inventoryItems.Add(new ItemSaveData
                {
                    itemName = stack.itemData.name,
                    amount = stack.amount
                });
            }
        }

        // 3. Подобранные предметы (PickupTracker)
        if (PickupTracker.Instance != null)
        {
            data.pickedUpItems = new List<string>(PickupTracker.Instance.GetPickedUpItems());
        }

        // 4. Уничтоженные объекты
        data.destroyedObjects = new List<string>(destroyedObjectIds);

        // 5. Состояния NPC
        SyncCurrentSceneNPCsToCache();
        data.npcStates = new List<NPCStateSaveData>(cachedNpcStates.Values);

        // Сериализация и запись
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveDataKey, json);
        PlayerPrefs.SetInt(SaveExistsKey, 1);
        PlayerPrefs.Save();

        Debug.Log("[SaveManager] Игра успешно сохранена.");
    }

    public void Load()
    {
        if (!HasSave()) return;

        string json = PlayerPrefs.GetString(SaveDataKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 1. Инвентарь
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearInventory();
            foreach (var itemData in data.inventoryItems)
            {
                ItemData item = Resources.Load<ItemData>($"Items/{itemData.itemName}");
                if (item != null)
                    InventoryManager.Instance.AddItem(item, itemData.amount);
            }
        }

        // 2. Подобранные предметы
        if (PickupTracker.Instance != null)
        {
            PickupTracker.Instance.LoadPickedUpItems(data.pickedUpItems);
        }

        // 3. Восстановление списка уничтоженных объектов
        destroyedObjectIds.Clear();
        if (data.destroyedObjects != null)
        {
            foreach (var id in data.destroyedObjects)
                destroyedObjectIds.Add(id);
        }

        // 4. Кэш NPC
        cachedNpcStates.Clear();
        foreach (var savedNpc in data.npcStates)
        {
            cachedNpcStates[savedNpc.stateName] = savedNpc;
        }

        // 5. Переход на сохраненную сцену
        Vector3 targetPosition = new Vector3(data.playerX, data.playerY, 0);
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneAtPosition(data.sceneName, targetPosition);
        }
        else
        {
            Debug.LogError("[SaveManager] SceneLoader.Instance не найден!");
        }

        Debug.Log($"[SaveManager] Загрузка инициирована: сцена {data.sceneName}");
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveExistsKey);
        PlayerPrefs.DeleteKey(SaveDataKey);
        PlayerPrefs.Save();
        cachedNpcStates.Clear();
        destroyedObjectIds.Clear();
    }

    // --- ОБРАБОТКА СЦЕНЫ ---

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyCachedStatesToSceneNPCs();
    }

    public void SyncCurrentSceneNPCsToCache()
    {
        NPCDialogue[] allNpcs = FindObjectsByType<NPCDialogue>(FindObjectsInactive.Include);
        foreach (var npc in allNpcs)
        {
            if (npc.State == null || string.IsNullOrEmpty(npc.NpcID)) continue;

            cachedNpcStates[npc.NpcID] = new NPCStateSaveData
            {
                stateName = npc.NpcID,
                isLoyal = npc.State.isLoyal,
                itemGiven = npc.State.itemGiven,
                isLocked = npc.State.isLocked
            };
        }
    }

    private void ApplyCachedStatesToSceneNPCs()
    {
        NPCDialogue[] allNpcs = FindObjectsByType<NPCDialogue>(FindObjectsInactive.Include);
        foreach (var npc in allNpcs)
        {
            if (string.IsNullOrEmpty(npc.NpcID) || npc.State == null) continue;

            if (cachedNpcStates.TryGetValue(npc.NpcID, out var savedState))
            {
                npc.State.isLoyal = savedState.isLoyal;
                npc.State.itemGiven = savedState.itemGiven;
                npc.State.isLocked = savedState.isLocked;
            }
        }
    }

    // --- МОДЕЛИ ДАННЫХ ---

    [System.Serializable]
    public class SaveData
    {
        public string sceneName;
        public float playerX;
        public float playerY;
        public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();
        public List<string> pickedUpItems = new List<string>();
        public List<string> destroyedObjects = new List<string>();
        public List<NPCStateSaveData> npcStates = new List<NPCStateSaveData>();
    }

    [System.Serializable]
    public class ItemSaveData
    {
        public string itemName;
        public int amount;
    }

    [System.Serializable]
    public class NPCStateSaveData
    {
        public string stateName;
        public bool isLoyal;
        public bool itemGiven;
        public bool isLocked;
    }
}