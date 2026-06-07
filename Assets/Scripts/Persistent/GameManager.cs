using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public sealed class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;
    private Transform playerTransform;

    public static GameManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {

        if (Instance != null) return;

        // Загружаем prefab GameManager из Resources
        var prefab = Resources.Load<GameObject>("GameManager");
        if (prefab != null)
            Instantiate(prefab);
        else
            Debug.LogError("Положи GameManager.prefab в Assets/Resources/");
        // if (Instance != null) return;

        // var go = new GameObject(nameof(GameManager));
        // go.AddComponent<GameManager>();
        // go.AddComponent<PickupTracker>();
        // go.AddComponent<InventoryManager>();
        // go.AddComponent<SaveManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Добавляем менеджеры как компоненты
        if (GetComponent<PickupTracker>() == null) gameObject.AddComponent<PickupTracker>();
        if (GetComponent<InventoryManager>() == null) gameObject.AddComponent<InventoryManager>();
        if (GetComponent<SaveManager>() == null) gameObject.AddComponent<SaveManager>();
        if (GetComponent<CombineManager>() == null) gameObject.AddComponent<CombineManager>();

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Главное меню — ничего не делаем
        if (scene.name == "Main menu") return;

        // Игрок уже существует (DontDestroyOnLoad) — не спавним повторно
        if (GameObject.FindWithTag("Player") == null)
            SpawnPlayer();

        if (Camera.main == null)
            SpawnCamera();
    }

    private void OnDestroy()
    {
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("playerPrefab не назначен!"); return;
        }
        var player = Instantiate(playerPrefab);
        player.name = "Player";
        player.tag = "Player";
        playerTransform = player.transform;
        DontDestroyOnLoad(player);
    }
    private void SpawnCamera()
    {
        if (cameraPrefab == null)
        {
            Debug.LogError("cameraPrefab не назначен!"); return;
        }
        var camera = Instantiate(cameraPrefab);
        camera.name = "Main Camera";
        Camera spawnedCamera = camera.GetComponentInChildren<Camera>();
        if (spawnedCamera != null)
            spawnedCamera.gameObject.tag = "MainCamera";
        else
            camera.tag = "MainCamera";

        CameraFollow cameraFollow = camera.GetComponentInChildren<CameraFollow>();
        if (cameraFollow != null)
            cameraFollow.SetTarget(playerTransform);

        DontDestroyOnLoad(camera);
    }
}
