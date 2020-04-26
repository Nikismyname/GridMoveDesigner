using UnityEngine;

public static class Utils
{
    public static Color RandColor()
    {
        Color result = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );

        return result;
    }

    public static void DrawX(Vector3 pos, Color color, float time = 1f)
    {
        Debug.DrawLine(pos.OffsetY(0.5f), pos.OffsetY(-0.5f), color, 0.3f);
        Debug.DrawLine(pos.OffsetX(0.5f), pos.OffsetX(-0.5f), color, 0.3f);
    }

    public static void DrawLine(Vector3 pos1, Vector3 pos2, Color color, float time = 1f)
    {
        Debug.DrawLine(pos1, pos2, color, 20);
    }

    public static void DrawEdgeLine(Vector3 pos1, Vector3 pos2, Color color, float offset = 0.03f, float time = 1f)
    {
        if (pos1.x == pos2.x)
        {
            Debug.DrawLine(pos1.OffsetX(offset), pos2.OffsetX(offset), color, time);
            Debug.DrawLine(pos1.OffsetX(-offset), pos2.OffsetX(-offset), color, time);
        }
        else
        if (pos1.y == pos2.y)
        {
            Debug.DrawLine(pos1.OffsetY(offset), pos2.OffsetY(offset), color, time);
            Debug.DrawLine(pos1.OffsetY(-offset), pos2.OffsetY(-offset), color, time);
        }
        else
        {
            Debug.Log("BIG PP");
        }
    }

    public static void DrawThickLine(Vector3 pos1, Vector3 pos2, Color color, float time = 1f)
    {
        float spread = 0.2f;
        int lineCount = 100;
        float offset = -spread / 2;

        if (pos1.x == pos2.x)
        {
            for (int i = 0; i < lineCount; i++)
            {
                Debug.DrawLine(pos1.OffsetX(offset), pos2.OffsetX(offset), color, time);
                offset += spread / lineCount;
            }
        }
        else
        if (pos1.y == pos2.y)
        {
            for (int i = 0; i < lineCount; i++)
            {
                Debug.DrawLine(pos1.OffsetY(offset), pos2.OffsetY(offset), color, time);
                offset += spread / lineCount;
            }
        }
        else
        {
            Debug.Log("BIG PP");
        }
    }
}

