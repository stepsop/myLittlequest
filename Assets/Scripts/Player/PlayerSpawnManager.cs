using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    // ID следующей точки спавна
    public static int NextSpawnID = 0;

    // Флаг — был ли переход между сценами
    // false = игра только запустилась, телепортировать не надо
    // true = был реальный переход, телепортируем игрока
    public static bool ShouldSpawn = false;

    [Header("ID этой точки спавна")]
    [SerializeField] private int spawnID = 0;

    // ЗАМЕНИ Start() НА ЭТО — временная версия с логами для диагностики:

    private void Start()
    {
        Debug.Log($"[SPAWN] Сцена={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, " +
           $"{gameObject.name} (spawnID={spawnID}) Start() вызван. " +
           $"ShouldSpawn={ShouldSpawn}, NextSpawnID={NextSpawnID}, " +
           $"МояПозиция={transform.position}, Time={Time.time}");

        if (!ShouldSpawn)
        {
            GameState.IsTransitioning = false;
            return;
        }

        if (spawnID != NextSpawnID) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("PlayerSpawnManager: Player не найден на сцене!");
            return;
        }

        Debug.Log($"[SPAWN] Сцена={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, " +
                   $"ДО телепортации: player.position={player.transform.position}, " +
                   $"spawnPoint.position={transform.position}");

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.position = transform.position;

        player.transform.position = transform.position;

        Debug.Log($"[SPAWN] Сцена={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, " +
                   $"ПОСЛЕ телепортации: player.position={player.transform.position}, " +
                   $"rb.position={(rb != null ? rb.position.ToString() : "null")}");

        ShouldSpawn = false;
        GameState.IsTransitioning = false;
    }

    private void LateUpdate()
    {
        // Временная диагностика: проверяем, не двигается ли игрок
        // ПОСЛЕ спавна в первые несколько кадров (признак гонки с физикой)
        if (Time.frameCount < 10)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                Debug.Log($"[SPAWN-WATCH] Сцена={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, Frame={Time.frameCount}, player.position={player.transform.position}");
        }
    }
}