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

        MovePlayerTo(player, target.transform.position);
    }

    // Ставит игрока на конкретные координаты — используется при загрузке сохранения,
    // где нет SpawnPoint с ID, а есть сырые X/Y из SaveData.
    public void SpawnAtPosition(Vector3 position)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("PlayerSpawnManager: Player не найден на сцене!");
            return;
        }

        MovePlayerTo(player, position);
    }

    // Общая логика перемещения — двигаем и Rigidbody2D, и transform,
    // иначе один кадр физика может "откатить" позицию назад.
    private void MovePlayerTo(GameObject player, Vector3 position)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.position = position;

        player.transform.position = position;
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

}