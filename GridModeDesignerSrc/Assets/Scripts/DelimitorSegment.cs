using Assets.Scripts.Helpers;
using UnityEngine;

public class DelimitorSegment : MonoBehaviour
{
    private Line line;
    private Camera camera;
    private bool isHorizontal;
    private float x { get; set; }
    private float y { get; set; }

    public void SetUp(Line line, Camera camera, bool isHorizontal, float dimention)
    {
        this.line = line;
        this.camera = camera;
        this.isHorizontal = isHorizontal;
        if (isHorizontal)
        {
            this.y = dimention;
        }
        else
        {
            this.x = dimention;
        }
    }

    private void OnMouseDrag()
    {
        this.MoveLine();
    }

    private void MoveLine()
    {
        Vector3 mp = Input.mousePosition;
        Vector3 pos = this.camera.GetComponent<Camera>().ScreenToWorldPoint(mp);

        if (this.isHorizontal)
        {
            this.line.MoveToNewY(pos.y);
            this.y = pos.y;
        }
        else
        {
            this.line.MoveToNewX(pos.x);
            this.x = pos.x;
        }
    }

    public bool isColliding(Vector2 pos)
    {
        if (this.isHorizontal)
        {
            if (Mathf.Abs(pos.y - this.y) < 0.1f)
            {
                return true;
            }
        }
        else
        {
            if (Mathf.Abs(pos.x - this.x) < 0.1f)
            {
                return true;
            }
        }

        return false;
    }
}

