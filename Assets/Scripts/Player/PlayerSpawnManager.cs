using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance { get; private set; }

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

    // Ставит игрока на SpawnPoint с нужным ID.
    // Перед этим убирает дубли Player, если они вдруг оказались в сцене.
    public void SpawnAtID(int spawnID)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("PlayerSpawnManager: Player не найден на сцене!");
            return;
        }

        SpawnPoint target = FindSpawnPoint(spawnID);
        if (target == null)
        {
            Debug.LogError($"PlayerSpawnManager: SpawnPoint с ID={spawnID} не найден в сцене " +
                            $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}!");
            return;
        }

        // Двигаем и Rigidbody2D (физическое тело), и transform —
        // иначе один кадр физика может "откатить" позицию назад.
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.position = target.transform.position;

        player.transform.position = target.transform.position;
    }

    // Находит все SpawnPoint в активной сцене и берёт нужный по ID.
    private SpawnPoint FindSpawnPoint(int spawnID)
    {
        SpawnPoint[] points = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (var point in points)
        {
            if (point.SpawnID == spawnID)
                return point;
        }
        return null;
    }

    // Гарантирует что в сцене остался ровно один Player.
    // Если по ошибке в новой сцене вручную стоит ещё один Player —
    // лишние уничтожаются, остаётся persistent-инстанс.
    // private GameObject GetSinglePlayer()
    // {
    //     GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    //     if (players.Length == 0)
    //         return null;

    //     if (players.Length > 1)
    //     {
    //         Debug.LogWarning($"PlayerSpawnManager: найдено {players.Length} объектов Player, " +
    //                           "удаляю дубли.");
    //         // Оставляем первый, остальные — в мусор.
    //         for (int i = 1; i < players.Length; i++)
    //             Destroy(players[i]);
    //     }

    //     return players[0];
    // }
}