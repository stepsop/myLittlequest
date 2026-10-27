using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class NeonOptionBorderGraphic : MaskableGraphic
{
    [SerializeField, Min(0f)] private float thickness = 2f;
    [SerializeField, Min(0f)] private float cornerRadius = 8f;
    [SerializeField, Range(1, 16)] private int cornerSegments = 8;

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        vertexHelper.Clear();

        Rect rect = rectTransform.rect;
        float halfWidth = rect.width * 0.5f;
        float halfHeight = rect.height * 0.5f;
        float radius = Mathf.Clamp(cornerRadius, 0f, Mathf.Min(halfWidth, halfHeight));
        float innerRadius = Mathf.Max(0f, radius - thickness);
        float inset = Mathf.Min(thickness, Mathf.Min(halfWidth, halfHeight));
        int segments = Mathf.Max(1, cornerSegments);
        int pointsPerCorner = segments + 1;
        var points = new System.Collections.Generic.List<Vector2>(pointsPerCorner * 4);

        AddCorner(points, new Vector2(halfWidth - radius, -halfHeight + radius), radius, -90f, segments);
        AddCorner(points, new Vector2(halfWidth - radius, halfHeight - radius), radius, 0f, segments);
        AddCorner(points, new Vector2(-halfWidth + radius, halfHeight - radius), radius, 90f, segments);
        AddCorner(points, new Vector2(-halfWidth + radius, -halfHeight + radius), radius, 180f, segments);

        for (int i = 0; i < points.Count; i++)
        {
            int corner = i / pointsPerCorner;
            Vector2 point = points[i];
            Vector2 center = CornerCenter(corner, halfWidth, halfHeight, radius);
            Vector2 innerPoint;
            if (radius > 0f)
            {
                innerPoint = center + (point - center).normalized * innerRadius;
            }
            else
            {
                float x = corner < 2 ? halfWidth - inset : -halfWidth + inset;
                float y = corner == 0 || corner == 3 ? -halfHeight + inset : halfHeight - inset;
                innerPoint = new Vector2(x, y);
            }

            vertexHelper.AddVert(point, color, Vector2.zero);
            vertexHelper.AddVert(innerPoint, color, Vector2.zero);
        }

        for (int i = 0; i < points.Count; i++)
        {
            int next = (i + 1) % points.Count;
            int outer = i * 2;
            int inner = outer + 1;
            int nextOuter = next * 2;
            int nextInner = nextOuter + 1;
            vertexHelper.AddTriangle(outer, nextOuter, nextInner);
            vertexHelper.AddTriangle(outer, nextInner, inner);
        }
    }

    private static void AddCorner(System.Collections.Generic.List<Vector2> points,
        Vector2 center, float radius, float startAngle, int segments)
    {
        for (int i = 0; i <= segments; i++)
        {
            float angle = (startAngle + 90f * i / segments) * Mathf.Deg2Rad;
            points.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
        }
    }

    private static Vector2 CornerCenter(int cornerIndex, float halfWidth, float halfHeight, float radius)
    {
        switch (cornerIndex)
        {
            case 0: return new Vector2(halfWidth - radius, -halfHeight + radius);
            case 1: return new Vector2(halfWidth - radius, halfHeight - radius);
            case 2: return new Vector2(-halfWidth + radius, halfHeight - radius);
            default: return new Vector2(-halfWidth + radius, -halfHeight + radius);
        }
    }
}
