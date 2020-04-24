using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToVec2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector3 ToVec3(this Vector2 vec, float y = 0)
    {
        return new Vector3(vec.x, y, vec.y);
    }

    public static bool AlmostTheSame(this Vector3 vec, Vector3 other, float threshold = 0.001f)
    {
        float distance = (vec - other).magnitude;

        if(distance< threshold)
        {
            return true; 
        }

        return false;
    }

    public static bool AlmostTheSame(this Vector2 vec, Vector2 other, float threshold = 0.001f)
    {
        float distance = (vec - other).magnitude;

        if (distance < threshold)
        {
            return true;
        }

        return false;
    }

    public static Vector3 OffsetX(this Vector3 vec, float xOffset)
    {
        return new Vector3(vec.x + xOffset, vec.y, vec.z);
    }

    public static Vector3 OffsetY(this Vector3 vec, float yOffset)
    {
        return new Vector3(vec.x, vec.y + yOffset, vec.z);
    }

    public static Vector3 OffsetZ(this Vector3 vec, float zOffset)
    {
        return new Vector3(vec.x, vec.y, vec.z + zOffset);
    }
}