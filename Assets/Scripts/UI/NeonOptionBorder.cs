using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Неоновая красная рамка варианта диалога: пульсация + редкие "сбои" лампы.
// Включается при наведении мыши или выборе с клавиатуры.
public class NeonOptionBorder : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("Рамка")]
    [SerializeField] private GameObject borderRoot;
    [SerializeField] private Graphic[] borderGraphics;
    [SerializeField] private Color neonColor = new Color(1f, 0.12f, 0.24f, 1f);

    [Header("Пульсация")]
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.55f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
    [SerializeField] private float pulseSpeed = 4f;

    [Header("Сбой неона")]
    [SerializeField, Range(0f, 5f)] private float glitchChancePerSecond = 0.6f;
    [SerializeField] private float glitchDuration = 0.07f;
    [SerializeField, Range(0f, 1f)] private float glitchAlpha = 0.1f;

    private Outline[] outlineEffects;
    private bool isPointerOver;
    private bool isSelected;
private float glitchTimer;

private void Awake()
    {
        if (borderRoot == null)
        {
            Transform borderTransform = transform.Find("NeonBorder");
            if (borderTransform != null)
                borderRoot = borderTransform.gameObject;
        }

        if (borderGraphics == null)
            borderGraphics = new Graphic[0];
        if (outlineEffects == null || outlineEffects.Length == 0)
            outlineEffects = GetComponentsInChildren<Outline>(true);

        SetBorderVisible(false);
    }

private void OnDisable()
    {
        isPointerOver = false;
        isSelected = false;
        SetBorderVisible(false);
    }

    private void Update()
    {
        if (borderRoot == null || !borderRoot.activeSelf)
        {
            return;
        }

        // unscaledTime — диалог может идти при timeScale = 0.
        float dt = Time.unscaledDeltaTime;

        if (glitchTimer > 0f)
        {
            glitchTimer -= dt;
        }
        else if (Random.value < glitchChancePerSecond * dt)
        {
            glitchTimer = glitchDuration;
        }

        float alpha = glitchTimer > 0f
            ? glitchAlpha
            : Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f);

        ApplyAlpha(alpha);
    }

public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        SetBorderVisible(true);
    }
public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        SetBorderVisible(isSelected);
    }
public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        SetBorderVisible(true);
    }
public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        SetBorderVisible(isPointerOver);
    }

private void SetBorderVisible(bool visible)
    {
        if (borderRoot == null)
            return;

        borderRoot.SetActive(visible);
        if (!visible)
            return;

        glitchTimer = 0f;
        ApplyAlpha(maxAlpha);
    }

private void ApplyAlpha(float alpha)
    {
        Color color = neonColor;
        color.a = alpha;

        if (borderGraphics != null)
        {
            foreach (Graphic graphic in borderGraphics)
            {
                if (graphic != null)
                    graphic.color = color;
            }
        }

        if (outlineEffects != null)
        {
            foreach (Outline outline in outlineEffects)
            {
                if (outline == null)
                    continue;

                Color effectColor = outline.effectColor;
                effectColor.a = alpha;
                outline.effectColor = effectColor;
            }
        }
    }
}
