using UnityEngine;
using System.Collections.Generic;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance != null) return;

        var go = new GameObject(nameof(GameManager));
        go.AddComponent<GameManager>();
        go.AddComponent<PickupTracker>();
        go.AddComponent<InventoryManager>();
        go.AddComponent<SaveManager>();
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
    }
}

