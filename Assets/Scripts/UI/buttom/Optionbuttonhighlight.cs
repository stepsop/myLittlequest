using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Подсветка фона варианта диалога при наведении + звук клика.
// Повесить на корень OptionButton.prefab.
public class OptionButtonHighlight : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("Подсветка")]
    [SerializeField, Range(0f, 1f)] private float highlightAlpha = 0.45f;

    [Header("Звук клика")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField, Range(0f, 3f)] private float clickVolume = 1f;

    [SerializeField] private Image targetImage;
    private Color normalColor;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
        if (targetImage != null)
        {
            normalColor = targetImage.color;
        }

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDisable()
    {
        
        SetHighlighted(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlighted(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetHighlighted(false);
    }

    private void SetHighlighted(bool highlighted)
    {
        if (targetImage == null)
        {
            return;
        }

        Color color = normalColor;
        color.a = highlighted ? highlightAlpha : normalColor.a;
        targetImage.color = color;
    }

    private void PlayClickSound()
    {
        if (clickSound == null)
        {
            return;
        }

        // Не AudioSource на кнопке: DialogueUI уничтожает кнопки сразу после клика и обрывает звук
        Camera cam = Camera.main;
        Vector3 position = cam != null ? cam.transform.position : Vector3.zero;
        AudioSource.PlayClipAtPoint(clickSound, position, clickVolume);
    }
}