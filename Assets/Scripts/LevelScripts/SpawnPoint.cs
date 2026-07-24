using UnityEngine;

// Просто точка с ID. Никакой логики поиска игрока —
// этим занимается PlayerSpawnController.
// Старый PlayerSpawnManager.cs удаляется, этот скрипт его заменяет.
public class SpawnPoint : MonoBehaviour
{
    [Header("ID этой точки спавна (совпадает с spawnPointID в SceneTransition)")]
    [SerializeField] private int spawnID = 0;

    public int SpawnID => spawnID;

    // Рисуем точку в редакторе для удобства — видно где именно появится игрок
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}