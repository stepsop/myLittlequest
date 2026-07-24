using UnityEngine;

// Вешается на любой источник света: фонарь, окно, неоновую вывеску.
// Мерцает яркостью случайным образом — создаёт атмосферу нуара.
public class FlickerLight : MonoBehaviour
{
    [Header("Диапазон яркости (0 = темно, 1 = полная яркость)")]
    [SerializeField] private float minBrightness = 0.4f;
    [SerializeField] private float maxBrightness = 1f;

    [Header("Скорость мерцания")]
    [SerializeField] private float minDelay = 0.05f;
    [SerializeField] private float maxDelay = 0.3f;

    // Плавность перехода между значениями яркости
    [SerializeField] private float smoothSpeed = 8f;

    private SpriteRenderer sr;
    private float targetBrightness;
    private float currentBrightness;
    private float timer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentBrightness = maxBrightness;
        targetBrightness = maxBrightness;
    }

    private void Update()
    {
        // Обратный отсчёт до следующей смены яркости
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Выбираем новую случайную яркость и время до следующей смены
            targetBrightness = Random.Range(minBrightness, maxBrightness);
            timer = Random.Range(minDelay, maxDelay);
        }

        // Плавно двигаемся к targetBrightness, а не дёргаемся резко
        currentBrightness = Mathf.Lerp(currentBrightness, targetBrightness, Time.deltaTime * smoothSpeed);

        // Применяем яркость через цвет спрайта (RGB остаются 1,1,1 — просто затемняем)
        Color c = sr.color;
        c.r = c.g = c.b = currentBrightness;
        sr.color = c;
    }
}