using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    public static readonly Dictionary<string, NPCDialogue> Registry = new();

    [SerializeField] private DialogueData startDialogue;
    private SpeechBubble speechBubble;

    [SerializeField] private NPCState npcStateTemplate;
    private NPCState npcState;
    [SerializeField] private string npcID;
    public NPCState State => npcState;
    public string NpcID => npcID;

    [SerializeField] private FailPhraseDatabase failPhrase;

    private bool playerInside;

    private void Awake()
    {
        // Если NPC был уничтожен ранее (через диалоговое действие DestroyObject)
        if (SaveManager.Instance != null && SaveManager.Instance.IsDestroyed(npcID))
        {
            Destroy(gameObject);
            return;
        }

        speechBubble = GetComponentInChildren<SpeechBubble>();
        npcState = npcStateTemplate != null ? npcStateTemplate.CreateInstance() : null;

        if (!string.IsNullOrEmpty(npcID)) 
            Registry[npcID] = this;
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(npcID)) 
            Registry.Remove(npcID);
    }

    public bool CanInteract()
    {
        return playerInside;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        // Если диалог заблокирован — NPC не разговаривает
        FailPhrase phrase = failPhrase != null ? failPhrase.GetRandom() : null;
        if (phrase != null)
        {
            speechBubble.Show(phrase.text, phrase.audio);
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