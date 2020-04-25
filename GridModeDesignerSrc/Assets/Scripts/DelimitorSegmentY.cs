using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelimitorSegmentY : MonoBehaviour
{
    public Line line;
    public Camera camera;
    public float Y { get; set; }
    public Vector2 left;
    public Vector2 right;
    public List<DelimitorSegmentX> others;
    public Main main;

    public void SetUp(
        Line line,
        List<DelimitorSegmentX> others,
        Camera camera,
        float dimention,
        Vector2 left,
        Vector2 right,
        Main main)
    {
        this.line = line;
        this.camera = camera;
        this.left = left;
        this.right = right;
        this.Y = dimention;
        this.others = others;
        this.main = main;

        if (left.x > right.x)
        {
            var temp = left;
            this.left = right;
            this.right = temp;
        }
    }

    private void OnMouseDrag()
    {
        this.MoveLine();
    }

    private void MoveLine()
    {
        if (this.main.mode != EditingModes.placingLines)
        {
            return;
        }

        Vector3 mp = Input.mousePosition;
        Vector3 pos = this.camera.GetComponent<Camera>().ScreenToWorldPoint(mp);

        this.line.MoveToNewY(pos.y);
        this.Y = pos.y;
    }

    public bool IsColliding(Vector2 pos)
    {
        if (Mathf.Abs(pos.y - this.Y) < 0.1f && pos.x >= this.left.x && pos.x <= this.right.x)
        {
            return true;
        }

        return false;
    }

    public BreakLinesResult[] BreakLine(Vector2 pos)
    {
        List<BreakLinesResult> result = new List<BreakLinesResult>();

        if (this.IsColliding(pos))
        {
            float[] intersectingXs = this.others
                .Where(x => x.top.y >= this.Y && x.bottom.y <= this.Y) /// vertical intersects this(hor) on y
                .Select(x => x.X)
                .ToArray();

            if (intersectingXs.Length > 0)
            {
                float left;
                float right;

                float[] leftXs = intersectingXs.Where(x => x < pos.x && x >= this.left.x).ToArray();
                if (leftXs.Length == 0)
                {
                    left = this.left.x;
                }
                else
                {
                    left = Mathf.Max(leftXs);
                }

                float[] rightXs = intersectingXs.Where(x => x >= pos.x && x <= this.right.x).ToArray();
                if (rightXs.Length == 0)
                {
                    right = this.right.x;
                }
                else
                {
                    right = Mathf.Min(rightXs);
                }

                if (this.left.x != left)
                {
                    result.Add(new BreakLinesResult
                    {
                        topLeft = new Vector2(this.left.x, this.Y),
                        bottomRight = new Vector2(left, this.Y),
                        dimention = this.Y,
                        isHorizontal = true,
                        camera = null,
                        line = null,
                        main = null,
                    });
                }

                if (this.right.x != right)
                {
                    result.Add(new BreakLinesResult
                    {
                        topLeft = new Vector2(right, this.Y),
                        bottomRight = new Vector2(this.right.x, this.Y),
                        dimention = this.Y,
                        isHorizontal = true,
                        camera = null,
                        line = null,
                        main = null,
                    });
                }
            }

            return result.ToArray();
        }

        return null;
    }

    public Segment[] Brake()
    {
        List<Segment> result = new List<Segment>();

        float[] intersectingXs = this.others
            .Where(x => x.top.y >= this.Y && x.bottom.y <= this.Y) /// vertical intersects this(hor) on y
            .Select(x => x.X)
            .Where(x => x >= this.left.x && x <= this.right.x)
            .OrderBy(x=>x)
            .ToArray();

        if (this.left.x != intersectingXs[0])
        {
            result.Add(new Segment
            {
                One = new Vector2(this.left.x, this.Y),
                Two = new Vector2(intersectingXs[0], this.Y),
                IsY = true
            });
        }

        for (int i = 0; i < intersectingXs.Length -1; i++)
        {
            result.Add(new Segment
            {
                One = new Vector2(intersectingXs[i], this.Y),
                Two = new Vector2(intersectingXs[i+1], this.Y),
                IsY = true
            });
        }

        if (this.right.x != intersectingXs[intersectingXs.Length-1])
        {
            result.Add(new Segment
            {
                One = new Vector2(intersectingXs[intersectingXs.Length - 1], this.Y),
                Two = new Vector2(this.right.x, this.Y),
                IsY = true
            });
        }

        return result.ToArray();
    }

    public void Destroy()
    {
        this.line.Destroy();
        Destroy(this.gameObject);
    }

    public void Hide()
    {
        this.line.line.SetActive(false);
    }

    public void Show()
    {
        this.line.line.SetActive(true);
    }
}

