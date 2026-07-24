using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Отвечает ТОЛЬКО за затемнение экрана (fade in/out).
// SceneLoader ничего не знает про Image — просто вызывает Fade(...).
public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.7f;

    // Плавно меняет прозрачность fadeImage от from до to.
    // from/to: 0 = прозрачно (видно игру), 1 = чёрный экран.
    public IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null)
        {
            Debug.LogError("FadeController: fadeImage не назначен.", this);
            yield break;
        }

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, to);
    }
}