using System.Collections;
using UnityEngine;

// Двигает RectTransform панели снизу вверх (и обратно) рывками,
// плюс проигрывает звук открытия/закрытия.
public class InventorySlideAnimation : MonoBehaviour
{
    [Header("Что двигаем")]
    [SerializeField] private RectTransform panelToMove;

    [Header("Параметры движения")]
    [SerializeField] private float hiddenOffsetY = 150f;
    [SerializeField] private int steps = 4;
    [SerializeField] private float stepDuration = 0.05f;

    [Header("Звук")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private Vector2 shownPos;
    private Vector2 hiddenPos;
    private Coroutine runningAnim;

    private void Awake()
    {
        shownPos = panelToMove.anchoredPosition;
        hiddenPos = shownPos + Vector2.down * hiddenOffsetY;

        // Просто сразу уводим панель вниз.
        // Объект НЕ выключаем.
        panelToMove.anchoredPosition = hiddenPos;
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
        Vector2 targetPos = toShown ? shownPos : hiddenPos;

        for (int i = 1; i <= steps; i++)
        {
            float t = (float)i / steps;
            panelToMove.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return new WaitForSeconds(stepDuration);
        }

        panelToMove.anchoredPosition = targetPos;
        runningAnim = null;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}