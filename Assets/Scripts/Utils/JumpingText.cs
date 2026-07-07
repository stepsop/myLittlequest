using UnityEngine;
using TMPro;

public class JumpingText : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float smoothSpeed = 12f;
    [SerializeField] private float minDelay = 0.05f;
    [SerializeField] private float maxDelay = 0.25f;

    private TextMeshProUGUI tmp;

    private float[] currentOffset;
    private float[] targetOffset;
    private float[] timer;

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
            currentOffset = new float[info.characterCount];
            targetOffset = new float[info.characterCount];
            timer = new float[info.characterCount];
        }

        for (int i = 0; i < info.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = info.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            timer[i] -= Time.deltaTime;

            if (timer[i] <= 0)
            {
                targetOffset[i] = Random.Range(-jumpHeight, jumpHeight);
                timer[i] = Random.Range(minDelay, maxDelay);
            }

            currentOffset[i] = Mathf.Lerp(
                currentOffset[i],
                targetOffset[i],
                Time.deltaTime * smoothSpeed);

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] verts = info.meshInfo[meshIndex].vertices;

            for (int j = 0; j < 4; j++)
                verts[vertexIndex + j] += Vector3.up * currentOffset[i];
        }

        for (int i = 0; i < info.meshInfo.Length; i++)
        {
            info.meshInfo[i].mesh.vertices = info.meshInfo[i].vertices;
            tmp.UpdateGeometry(info.meshInfo[i].mesh, i);
        }
    }
}