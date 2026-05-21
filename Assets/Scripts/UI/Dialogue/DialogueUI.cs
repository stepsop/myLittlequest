using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("Главное окно")]
    [SerializeField] private GameObject dialogueCanvas;      // DialogueCanvas
    [SerializeField] private GameObject optionsCanvas;       // OptionsDialogueCanvas ← новое

    [Header("Игрок — левая сторона")]
    [SerializeField] private Image playerPortraitImage;
    [SerializeField] private TMP_Text playerNameText;

    [Header("НПС — правая сторона")]
    [SerializeField] private Image npcPortraitImage;
    [SerializeField] private TMP_Text npcNameText;

    [Header("Диалог")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Transform optionsContainer;     // OptionsContainer внутри OptionsCanvas
    [SerializeField] private Button optionPrefab;

    [Header("Скорость печати")]
    [SerializeField] private float typewriterSpeed = 0.03f;

    private Coroutine typewriterCoroutine;
    private bool isTyping = false;
    private DialogueData currentDialogue;
    private PlayerInputActions input;

    private void Awake()
    {
        input = new PlayerInputActions();
        Instance = this; // Это работает только если объект активен при старте

        // Input System: пока asset не включён, WasPressedThisFrame() никогда не сработает.
        // Мы включаем его здесь, чтобы диалог гарантированно реагировал на Interact,
        // даже если другие части игры используют свои экземпляры PlayerInputActions.
        input.Enable();

        // Канвасы скрываем через SetActive — сам объект DialogueUI должен быть активен!
        dialogueCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        // Корректно отключаем input, чтобы не оставлять включённые action maps после уничтожения UI.
        // (Особенно важно, если кто-то всё-таки решит пересоздавать DialogueUI при смене сцен.)
        input?.Disable();
    }

    public void OpenDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        GameState.IsDialogueOpen = true;

        dialogueCanvas.SetActive(true);
        optionsCanvas.SetActive(true); // Кнопки скрыты пока текст печатается

        // Портрет и имя игрока
        if (playerNameText != null)
            playerNameText.text = dialogue.playerName;
        if (playerPortraitImage != null)
        {
            playerPortraitImage.sprite = dialogue.playerPortrait;
            playerPortraitImage.gameObject.SetActive(dialogue.playerPortrait != null);
        }

        // Портрет и имя NPC
        if (npcNameText != null)
            npcNameText.text = dialogue.npcName;
        if (npcPortraitImage != null)
        {
            npcPortraitImage.sprite = dialogue.npcPortrait;
            npcPortraitImage.gameObject.SetActive(dialogue.npcPortrait != null);
        }

        // Запускаем печать текста
        ClearOptions();
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        typewriterCoroutine = StartCoroutine(TypeText(dialogue));
    }

    private IEnumerator TypeText(DialogueData dialogue)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in dialogue.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        SpawnOptions(dialogue); // Текст допечатан — показываем кнопки
    }

    private void Update()
    {
        // Пропустить анимацию печати по нажатию E или Space
        if (GameState.IsDialogueOpen && isTyping)
        {
            if (input.Player.Interact.WasPressedThisFrame())
            {
                StopCoroutine(typewriterCoroutine);
                isTyping = false;
                dialogueText.text = currentDialogue.text;
                SpawnOptions(currentDialogue);
            }
        }
    }

    private void SpawnOptions(DialogueData dialogue)
    {
        ClearOptions();
        optionsCanvas.SetActive(true);

        foreach (var option in dialogue.options)
        {
            // Проверяем условие — показывать ли этот вариант
            if (!CheckCondition(option)) continue;

            Button btn = Instantiate(optionPrefab, optionsContainer);
            btn.GetComponentInChildren<TMP_Text>().text = option.text;

            var localOption = option;
            btn.onClick.AddListener(() =>
            {
                // Выполняем действие при выборе
                ExecuteAction(localOption);

                if (localOption.nextDialogue != null)
                    OpenDialogue(localOption.nextDialogue);
                else
                    CloseDialogue();
            });
        }
    }
    private bool CheckCondition(DialogueOption option)
    {
        switch (option.conditionType)
        {
            case ConditionType.None:
                // Скрываем вариант если предмет уже выдан
                if (option.actionType == ActionType.GiveItem &&
                    option.targetNpcState != null &&
                    option.targetNpcState.itemGiven)
                    return false;
                return true;

            case ConditionType.HasItem:
                return option.requiredItem != null &&
                       InventoryManager.Instance.HasItem(option.requiredItem);

            case ConditionType.NPCIsLoyal:
                return option.npcState != null && option.npcState.isLoyal;

            case ConditionType.NPCNotLoyal:
                return option.npcState != null && !option.npcState.isLoyal;

            default:
                return true;
        }
    }
    private void ExecuteAction(DialogueOption option)
    {
        switch (option.actionType)
        {
            case ActionType.GiveItem:
                if (option.giveItem != null && option.targetNpcState != null
                    && !option.targetNpcState.itemGiven)
                {
                    InventoryManager.Instance.AddItem(option.giveItem);
                    option.targetNpcState.itemGiven = true;
                }
                break;

            case ActionType.TakeItem:
                // NPC забирает предмет у игрока
                if (option.takeItem != null)
                    InventoryManager.Instance.RemoveItem(option.takeItem);
                break;

            case ActionType.ExchangeItems:
                // Обмен — забирает takeItem, выдаёт giveItem
                if (option.takeItem != null)
                    InventoryManager.Instance.RemoveItem(option.takeItem);
                if (option.giveItem != null && option.targetNpcState != null
                    && !option.targetNpcState.itemGiven)
                {
                    InventoryManager.Instance.AddItem(option.giveItem);
                    option.targetNpcState.itemGiven = true;
                }
                break;

            case ActionType.SetLoyal:
                // NPC становится лояльным
                if (option.targetNpcState != null)
                    option.targetNpcState.isLoyal = true;
                break;

            case ActionType.LockDialogue:
                // Блокируем диалог навсегда
                if (option.targetNpcState != null)
                    option.targetNpcState.isLocked = true;
                break;
        }
        if (option.lockDialogue && option.targetNpcState != null)
            option.targetNpcState.isLocked = true;
    }

    private void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);
    }

    public void CloseDialogue()
    {
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        isTyping = false;
        GameState.IsDialogueOpen = false;
        ClearOptions();
        dialogueCanvas.SetActive(false);
        optionsCanvas.SetActive(false); // Прячем оба канваса
    }
}