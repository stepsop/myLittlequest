using System.Collections;
using UnityEngine;

// Двигает RectTransform панели снизу вверх (и обратно) рывками,
// плюс проигрывает звук открытия/закрытия.
public class InventorySlideAnimation : MonoBehaviour
{
    [Header("Что двигаем")]
    [SerializeField] private RectTransform panelToMove;
    [SerializeField] private RectTransform buttonToMove;

    [Header("Параметры движения")]
    [SerializeField] private float hiddenOffsetY = 150f;
    [SerializeField] private int steps = 4;
    [SerializeField] private float stepDuration = 0.05f;

    [Header("Звук")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private Vector2 shownPosPanel;
    private Vector2 hiddenPosPanel;

    private Vector2 shownPosButton;
    private Vector2 hiddenPosButton;
    private Coroutine runningAnim;

    private void Awake()
    {
        shownPosPanel = panelToMove.anchoredPosition;
        hiddenPosPanel = shownPosPanel + Vector2.down * hiddenOffsetY;

        shownPosButton = buttonToMove.anchoredPosition;
        hiddenPosButton = shownPosButton + Vector2.down * hiddenOffsetY;

        // Просто сразу уводим панель вниз.
        // Объект НЕ выключаем.
        panelToMove.anchoredPosition = hiddenPosPanel;
        buttonToMove.anchoredPosition = hiddenPosButton;
    }

    public void Show()
    {
        PlaySound(openSound);
        RestartAnimation(true);
    }

    public void Hide()
    {
        PlaySound(closeSound);
        RestartAnimation(false);
    }

    private void RestartAnimation(bool toShown)
    {
        if (runningAnim != null)
            StopCoroutine(runningAnim);

        runningAnim = StartCoroutine(AnimateSteps(toShown));
    }

    private IEnumerator AnimateSteps(bool toShown)
    {
        Vector2 startPos = panelToMove.anchoredPosition;
        Vector2 targetPos = toShown ? shownPosPanel : hiddenPosPanel;
        Vector2 startButtonPos = buttonToMove.anchoredPosition;
        Vector2 targetButtonPos = toShown ? shownPosButton : hiddenPosButton;

        for (int i = 1; i <= steps; i++)
        {
            float t = (float)i / steps;
            panelToMove.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            buttonToMove.anchoredPosition = Vector2.Lerp(startButtonPos, targetButtonPos, t);
            yield return new WaitForSeconds(stepDuration);
        }

        panelToMove.anchoredPosition = targetPos;
        buttonToMove.anchoredPosition = targetButtonPos;
        runningAnim = null;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}