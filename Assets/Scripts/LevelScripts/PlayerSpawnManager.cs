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

    private void Start()
    {
        // Если перехода не было — ничего не делаем
        // Игрок стоит там где поставлен в редакторе
        if (!ShouldSpawn) return;

        PlayerSpawnManager[] allSpawns = FindObjectsByType<PlayerSpawnManager>(FindObjectsSortMode.None);

        foreach (var spawn in allSpawns)
        {
            if (spawn.spawnID == NextSpawnID)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                    player.transform.position = spawn.transform.position;

                break;
            }
        }

        // Сбрасываем флаг после телепортации
        ShouldSpawn = false;
    }
}