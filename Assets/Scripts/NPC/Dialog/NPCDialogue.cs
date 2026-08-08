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
       

        
        DialogueUI ui = DialogueUI.Instance;
        if (ui == null)
        {
           
            ui = Object.FindAnyObjectByType<DialogueUI>(FindObjectsInactive.Include);
        }

        if (ui == null)
        {
            return;
        }

        if (!ui.gameObject.activeInHierarchy)
        {
          
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