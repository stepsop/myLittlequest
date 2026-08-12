using UnityEngine;
using TMPro;

public class JumpingText : MonoBehaviour
{
    [Header("Amplitude")]
    [Range(0f, 20f)]
    [SerializeField] private float amplitudeX = 3f;          // горизонтальный размах
    
    [Range(0f, 20f)]
    [SerializeField] private float amplitudeY = 5f;          // вертикальный размах

    [Header("Noise & Speed")]
    [Range(0.1f, 5f)]
    [SerializeField] private float noiseStrength = 1.8f;     // сила шума (больше = хаотичнее)

    [Range(1f, 40f)]
    [SerializeField] private float smoothSpeed = 18f;        // скорость следования за целью

    [Range(0.005f, 0.3f)]
    [SerializeField] private float minDelay = 0.02f;         // минимальная пауза между сменами цели

    [Range(0.01f, 0.5f)]
    [SerializeField] private float maxDelay = 0.12f;         // максимальная пауза

    [Header("Extra")]
    [SerializeField] private bool usePerlinNoise = true;     // более "живой" шум

    [Range(1f, 30f)]
    [SerializeField] private float perlinScale = 8f;         // масштаб Perlin (выше = быстрее меняется)

    private TextMeshProUGUI tmp;
    private Vector2[] currentOffset;
    private Vector2[] targetOffset;
    private float[] timer;
    private float[] noiseSeed;                               // индивидуальный seed для каждой буквы

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        tmp.ForceMeshUpdate();
        TMP_TextInfo info = tmp.textInfo;

        if (currentOffset == null || currentOffset.Length != info.characterCount)
        {
            int count = info.characterCount;
            currentOffset = new Vector2[count];
            targetOffset = new Vector2[count];
            timer = new float[count];
            noiseSeed = new float[count];

            for (int i = 0; i < count; i++)
            {
                noiseSeed[i] = Random.Range(0f, 1000f);
                timer[i] = Random.Range(0f, maxDelay);
            }
        }

        for (int i = 0; i < info.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = info.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            timer[i] -= Time.deltaTime;

            if (timer[i] <= 0f)
            {
                if (usePerlinNoise)
                {
                    float t = Time.time * perlinScale + noiseSeed[i];
                    float nx = (Mathf.PerlinNoise(t, noiseSeed[i]) - 0.5f) * 2f;
                    float ny = (Mathf.PerlinNoise(noiseSeed[i], t) - 0.5f) * 2f;

                    // добавляем дополнительный случайный шум
                    nx += Random.Range(-1f, 1f) * noiseStrength * 0.4f;
                    ny += Random.Range(-1f, 1f) * noiseStrength * 0.4f;

                    targetOffset[i] = new Vector2(
                        nx * amplitudeX,
                        ny * amplitudeY
                    );
                }
                else
                {
                    // чистый рандом (более дёрганый)
                    targetOffset[i] = new Vector2(
                        Random.Range(-amplitudeX, amplitudeX) * noiseStrength,
                        Random.Range(-amplitudeY, amplitudeY) * noiseStrength
                    );
                }

                timer[i] = Random.Range(minDelay, maxDelay);
            }

            // плавно подтягиваем текущее смещение к цели
            currentOffset[i] = Vector2.Lerp(
                currentOffset[i],
                targetOffset[i],
                Time.deltaTime * smoothSpeed
            );

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Vector3[] verts = info.meshInfo[meshIndex].vertices;

            Vector3 offset = new Vector3(currentOffset[i].x, currentOffset[i].y, 0f);

            for (int j = 0; j < 4; j++)
                verts[vertexIndex + j] += offset;
        }

        for (int i = 0; i < info.meshInfo.Length; i++)
        {
            info.meshInfo[i].mesh.vertices = info.meshInfo[i].vertices;
            tmp.UpdateGeometry(info.meshInfo[i].mesh, i);
        }
    }
}