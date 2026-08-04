using UnityEngine;
using UnityEngine.Rendering.Universal; // нужно для Light2D

// Мерцание источника света Light2D (URP) с эффектом "поломки" —
// редкие резкие затемнения/отключения поверх плавного дрожания.
public class FlickerLight2D : MonoBehaviour
{
    [Header("Обычное дрожание (естественное мерцание лампы)")]
    [SerializeField] [Range(0f, 10f)] private float minIntensity = 1.5f;
    [SerializeField] [Range(0f, 10f)] private float maxIntensity = 3f;
    [SerializeField] [Range(0f, 10f)] private float minDelay = 0.05f;
    [SerializeField] [Range(0f, 10f)] private float maxDelay = 0.3f;
    [SerializeField] [Range(0f, 10f)] private float smoothSpeed = 8f;

    [Header("Глитч (эффект поломки)")]
    // Шанс, что в течение секунды случится глитч (0.1 = примерно раз в 10 сек)
    [SerializeField] [Range(0f, 1f)] private float glitchChance = 0.1f;

    // Насколько сильно гаснет свет во время глитча (0 = полностью гаснет, 1 = не гаснет)
    [SerializeField] [Range(0f, 1f)] private float glitchIntensity = 0.05f;

    // Сколько длится один глитч-провал
    [SerializeField] [Range(0f, 1f)] private float glitchDuration = 0.08f;

    private Light2D light2D;
    private float targetIntensity;
    private float currentIntensity;
    private float timer;

    private bool isGlitching;
    private float glitchTimer;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        currentIntensity = maxIntensity;
        targetIntensity = maxIntensity;
    }

    private void Update()
    {
        HandleGlitchTrigger();

        if (isGlitching)
        {
            HandleGlitch();
        }
        else
        {
            HandleNormalFlicker();
        }
    }

    // Проверяем — не пора ли запустить случайный глитч
    private void HandleGlitchTrigger()
    {
        if (isGlitching) return;

        // Проверка "раз в кадр" с учётом Time.deltaTime,
        // чтобы шанс не зависел от FPS
        if (Random.value < glitchChance * Time.deltaTime)
        {
            isGlitching = true;
            glitchTimer = glitchDuration;
        }
    }

    // Пока идёт глитч — резко проваливаем яркость почти до нуля
    private void HandleGlitch()
    {
        glitchTimer -= Time.deltaTime;

        // Резкий провал яркости (без плавности — это и создаёт эффект "сбоя")
        currentIntensity = maxIntensity * glitchIntensity;
        light2D.intensity = currentIntensity;

        if (glitchTimer <= 0f)
        {
            isGlitching = false;
        }
    }

    // Обычное плавное дрожание — как раньше
    private void HandleNormalFlicker()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            timer = Random.Range(minDelay, maxDelay);
        }

        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * smoothSpeed);
        light2D.intensity = currentIntensity;
    }
}