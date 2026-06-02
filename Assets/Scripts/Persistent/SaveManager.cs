using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Отвечает за сохранение и загрузку всего состояния игры.
// Использует PlayerPrefs + JSON — просто и без внешних зависимостей.
// Один слот сохранения на всю игру.
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private float _loadedPlayerX;
    private float _loadedPlayerY;
    private bool _pendingPositionRestore = false;

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
        NPCState[] allStates = Resources.FindObjectsOfTypeAll<NPCState>();
        foreach (var state in allStates)
        {
            data.npcStates.Add(new NPCStateSaveData
            {
                stateName = state.name, // Имя SO asset файла
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

    // Загружаем состояние игры
    public void Load()
    {
        if (!HasSave()) return;

        string json = PlayerPrefs.GetString(SaveDataKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 1. Восстанавливаем инвентарь
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
        NPCState[] allStates = Resources.FindObjectsOfTypeAll<NPCState>();
        foreach (var savedState in data.npcStates)
            foreach (var state in allStates)
                if (state.name == savedState.stateName)
                {
                    state.isLoyal = savedState.isLoyal;
                    state.itemGiven = savedState.itemGiven;
                    state.isLocked = savedState.isLocked;
                    break;
                }

        // 4. Сохраняем позицию — загрузим её ПОСЛЕ перехода на сцену
        _loadedPlayerX = data.playerX;
        _loadedPlayerY = data.playerY;
        _pendingPositionRestore = true;

        // Ждём загрузки и ставим позицию
        ResetGameplayState();

        // 5. Загружаем сцену
        SceneManager.LoadScene(data.sceneName);

        Debug.Log($"Игра загружена: сцена {data.sceneName}");

        // Загружаем сцену — это должно быть последним действием
        // После LoadScene Unity перезагрузит объекты,
        // поэтому позицию игрока нужно восстанавливать через PlayerSpawnManager
    }

    // Удаляем сохранение — при новой игре
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveExistsKey);
        PlayerPrefs.DeleteKey(SaveDataKey);
        PlayerPrefs.Save();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!_pendingPositionRestore) return;

        _pendingPositionRestore = false;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.transform.position = new Vector3(_loadedPlayerX, _loadedPlayerY, 0);

        ResetGameplayState();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private IEnumerator SetPositionAfterLoad(float x, float y)
    {
        // Ждём один кадр — сцена должна загрузиться
        yield return null;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.transform.position = new Vector3(x, y, 0);

    }

    // Контейнер всех данных для сохранения
    // [System.Serializable] нужен чтобы JsonUtility мог сериализовать класс
    private static void ResetGameplayState()
    {
        Time.timeScale = 1f;
        GameState.IsDialogueOpen = false;
        GameState.IsInventoryOpen = false;
        GameState.IsTransitioning = false;
        GameState.IsMenuOpen = false;
        GameState.IsInspecting = false;
    }

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
