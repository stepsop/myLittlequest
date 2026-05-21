using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData startDialogue;

    [SerializeField] private NPCState npcState;

    [SerializeField] private string lockedPhrase = "Я занят, уходи.";
    [SerializeField] private AudioClip lockedAudio;

    private bool playerInside;

    public bool CanInteract()
    {
        return playerInside;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        // Если диалог заблокирован — NPC не разговаривает
        if (npcState != null && npcState.isLocked)
        {
            SpeechBubble.Instance?.Show(lockedPhrase, lockedAudio);
            return;
        }
        if (!CanInteract()) return;

        // DialogueUI.Instance становится null, если объект с DialogueUI выключен в инспекторе:
        // Unity НЕ вызывает Awake() на неактивных объектах => Instance не присваивается.
        //
        // Поэтому мы делаем "мягкий автоподъём":
        // 1) пробуем Instance
        // 2) если null — ищем DialogueUI даже среди неактивных объектов
        // 3) если нашли, но он выключен — включаем (Unity вызовет Awake и он сам выставит Instance)
        DialogueUI ui = DialogueUI.Instance;
        if (ui == null)
        {
            // FindObjectsInactive.Include позволяет найти объект даже если его GameObject выключен.
            ui = Object.FindAnyObjectByType<DialogueUI>(FindObjectsInactive.Include);
        }

        if (ui == null)
        {
            Debug.LogError("DialogueUI не найден в сцене. Либо добавь DialogueUI на сцену, либо убедись что он существует в префабе UI.");
            return;
        }

        if (!ui.gameObject.activeInHierarchy)
        {
            // Важно: если DialogueUI лежит внутри выключенного родителя (например, весь UI-контейнер),
            // то SetActive(true) только на самом DialogueWindow НЕ поможет:
            // активность в иерархии = activeSelf && active всех родителей.
            //
            // Поэтому поднимаем вверх по Transform-цепочке и включаем всех отключённых родителей.
            Transform t = ui.transform;
            while (t != null)
            {
                if (!t.gameObject.activeSelf)
                    t.gameObject.SetActive(true);

                t = t.parent;
            }
        }

        ui.OpenDialogue(startDialogue);
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