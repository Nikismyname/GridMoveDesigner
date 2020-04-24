using UnityEngine;
using UnityEngine.UI;

public class UIColorHelper : MonoBehaviour
{
    private Color originalColor;
    private Image image;

    private void Start()
    {
        this.image = gameObject.GetComponent<Image>();
        this.originalColor = image.color;
    }

    public void ResetColor()
    {
        image.color = this.originalColor;
    }

    public void ChangeOriginalColor(Color col)
    {
        this.originalColor = col;
    }

    public void SetColor(Color col)
    {
        this.image.color = col;
    }
}