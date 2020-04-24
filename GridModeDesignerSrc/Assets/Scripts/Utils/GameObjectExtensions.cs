using UnityEngine;
using UnityEngine.UI;

public static class GameObjectExtensions
{
    const string DefaultShader = "Unlit/Color";

    public static void SetShader(this GameObject go, string shader = DefaultShader)
    {
        go.GetComponent<Renderer>().material.shader = Shader.Find(shader);
    }

    public static void SetColor(this GameObject go, Color col)
    {
        go.GetComponent<Renderer>().material.color = col;
    }

    public static void SetPos(this GameObject go, Vector3 pos)
    {
        go.transform.position = pos;
    }

    public static void SetScale(this GameObject go, Vector3 scale)
    {
        go.transform.localScale = scale; 
    }

    public static void SetScale(this GameObject go, float scale)
    {
        go.transform.localScale = new Vector3(scale, scale, scale);
    }

    public static float GetScale(this GameObject go)
    {
        Vector3 ls = go.transform.localScale;
        if(ls.x != ls.y || ls.y != ls.z)
        {
            Debug.LogWarning("The scale your are trying to access by using go.GetScale is not uniform!"); 
        }

        return go.transform.localScale.x;
    }

    public static Vector3 GetPos(this GameObject go)
    {
        return go.transform.position;
    }

    public static void OffsetPos(this GameObject go, Vector3 pos)
    {
        go.transform.position += pos;
    }

    ///RectTransform
    public static void SetRTSize(this GameObject go, float x, float y)
    {
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
    }
    public static void SetRTSize(this GameObject go, Vector2 vec)
    {
        go.GetComponent<RectTransform>().sizeDelta = vec;
    }
    public static void SetRTPos(this GameObject go, float x, float y)
    {
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }
    public static void SetRTPos(this GameObject go, Vector2 vec)
    {
        go.GetComponent<RectTransform>().anchoredPosition = vec;
    }
    ///...
    
    ///TODO: Clean this up!
    public static void SetImageColor(this GameObject go, Color col)
    {
        bool usedSecondImage = false;
        bool foundImage = false;

        Image image = go.GetComponent<Image>();

        if (image == null)
        {
            usedSecondImage = true;
        }
        else
        {
            foundImage = true;
        }

        if (foundImage == false)
        {
            image = go.GetComponentInChildren<Image>();
        }

        if(image != null)
        {
            foundImage = true;
        }

        if (foundImage)
        {
            image.color = col;

            if (usedSecondImage)
            {
                Debug.LogWarning("Image In Children Used!");
            }
        }
    }

    public static void SetImageMaterialColor(this GameObject go, Color col)
    {
        go.GetComponent<Image>().material.color = col;
    }

    public static void SetImageMaterialColorThroughHelper(this GameObject go, Color col)
    {
        UIColorHelper helper = go.GetComponent<UIColorHelper>();
        helper.SetColor(col);
    }

    public static void ResetImageMaterialColorThroughHelper(this GameObject go, Color col)
    {
        UIColorHelper helper = go.GetComponent<UIColorHelper>();
        helper.ResetColor();
    }
}