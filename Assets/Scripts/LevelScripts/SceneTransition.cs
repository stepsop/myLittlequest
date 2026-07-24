using UnityEngine;

// Используем IInteractable — тот же интерфейс что у NPC и предметов
// Это значит что PlayerMovement автоматически найдёт дверь через TryInteract()
// и не нужно писать отдельную логику для E
public class SceneTransition : MonoBehaviour, IInteractable
{
    [Header("Куда переходим")]
    [SerializeField] private string targetScene;

    [Header("ID точки спавна в новой сцене")]
    [SerializeField] private int spawnPointID = 0;

    // Флаг — игрок рядом с дверью
    private bool playerInside = false;

    public bool CanInteract()
    {
        return playerInside;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        Debug.Log($"[TRANSITION] Переход: targetScene={targetScene}, spawnPointID={spawnPointID}");

        // SceneLoader.Instance может быть "битым" (уничтоженный объект) если в сценах были дубли
       
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader.Instance == null. Добавь SceneLoader в GameManager prefab.");
            return;
        }

        // spawnPointID передаётся напрямую в LoadScene — без статических флагов.
        SceneLoader.Instance.LoadScene(targetScene, spawnPointID);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

}