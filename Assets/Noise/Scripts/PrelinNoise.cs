using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrelinNoise
{
    private static float GetEaseCurves(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static Vector2 GetEaseCurves(Vector2 p)
    {
        return new Vector2(GetEaseCurves(p.x), GetEaseCurves(p.y));
    }

    private static Vector2 Floor(Vector2 p)
    {
        return new Vector2(Mathf.Floor(p.x), Mathf.Floor(p.y));
    }

    private static Vector2 Fract(Vector2 p)
    {
        return p - Floor(p);
    }

    private static Vector2 Sin(Vector2 p)
    {
        return new Vector2(Mathf.Sin(p.x), Mathf.Sin(p.y));
    }

    private static Vector2 Hash22(Vector2 p)
    {
        p = new Vector2(Vector2.Dot(p, new Vector2(127.1f, 311.7f)),
                        Vector2.Dot(p, new Vector2(269.5f, 183.3f)));

        return new Vector2(-1, -1) + 2.0f * Fract(Sin(p) * 43758.5453123f);
    }

    public static float prelin_noise(Vector2 p)
    {
        Vector2 pi = Floor(p);
        Vector2 pf = p - pi;

        Vector2 w = GetEaseCurves(pf);

        float corner1 = Vector2.Dot(Hash22(pi + Vector2.zero), pf - Vector2.zero);
        float corner2 = Vector2.Dot(Hash22(pi + Vector2.right), pf - Vector2.right);
        float corner3 = Vector2.Dot(Hash22(pi + Vector2.up), pf - Vector2.up);
        float corner4 = Vector2.Dot(Hash22(pi + Vector2.one), pf - Vector2.one);

        return Mathf.Lerp(Mathf.Lerp(corner1, corner2, w.x),
                          Mathf.Lerp(corner3, corner4, w.x),
                          w.y);
    }

}
