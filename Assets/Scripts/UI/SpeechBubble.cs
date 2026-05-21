using System.Collections;
using UnityEngine;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    public static SpeechBubble Instance { get; private set; }

    [SerializeField] private GameObject bubblePanel;
    [SerializeField] private TextMeshProUGUI bubbleText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float typewriterSpeed = 0.03f; // Скорость печати

    private Coroutine hideCoroutine;
    private Coroutine typeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        bubblePanel.SetActive(false);
    }

    public void Show(string text, AudioClip clip = null)
    {
        if (string.IsNullOrEmpty(text)) return;

        bubblePanel.SetActive(true);

        // Останавливаем предыдущие корутины если облачко уже показывается
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);

        typeCoroutine = StartCoroutine(TypeText(text));

        if (clip != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private IEnumerator TypeText(string text)
    {
        bubbleText.text = "";

        foreach (char c in text)
        {
            bubbleText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        // Текст допечатан — запускаем таймер скрытия
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        bubblePanel.SetActive(false);
    }
}