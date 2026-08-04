using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public sealed class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject uiRootPrefab;
    private bool uiRootSpawned = false;

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


        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private void SpawnUIRoot()
    {
        if (uiRootSpawned) return;

        if (uiRootPrefab == null)
        {
            Debug.LogError("uiRootPrefab не назначен в GameManager!");
            return;
        }

        var uiRoot = Instantiate(uiRootPrefab);
        uiRoot.name = "UIRoot";
        DontDestroyOnLoad(uiRoot);
        uiRootSpawned = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        SpawnUIRoot();
    }

    private void OnDestroy()
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


}