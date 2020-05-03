using Assets.Scripts.Helpers;
using RecRec;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelimitorSegmentX : MonoBehaviour
{
    public Line line;
    public Camera camera;
    public float X { get; set; }
    public Vector2 top;
    public Vector2 bottom;
    public List<DelimitorSegmentY> others;
    public Main main;

    public void SetUp(
        Line line,
        List<DelimitorSegmentY> others,
        Camera camera,
        float dimention,
        Vector2 top,
        Vector2 bottom,
        Main main)
    {
        this.line = line;
        this.camera = camera;
        this.bottom = bottom;
        this.top = top;
        this.X = dimention;
        this.others = others;
        this.main = main;

        if(this.top.y < this.bottom.y)
        {
            var temp = this.bottom; 
            this.bottom = this.top;
            this.top = temp;
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
        this.line.MoveToNewX(pos.x);
        this.X = pos.x;
    }

    public bool IsColliding(Vector2 pos)
    {

        if (Mathf.Abs(pos.x - this.X) < 0.1f && pos.y >= this.bottom.y && pos.y <= this.top.y)
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
            float[] intersectingYs = this.others
                .Where(x => x.left.x <= this.X && x.right.x >= this.X)
                .Select(x => x.Y)
                .ToArray();

            if (intersectingYs.Length > 0)
            {
                float bottom;
                float top;

                float[] topYs = intersectingYs.Where(x => x > pos.y && x <= this.top.y ).ToArray();
                if (topYs.Length == 0)
                {
                    top = this.top.y;
                }
                else
                {
                    top = Mathf.Min(topYs);
                }

                float[] bottomYs = intersectingYs.Where(x => x <= pos.y && x >= this.bottom.y).ToArray();
                if (bottomYs.Length == 0)
                {
                    bottom = this.bottom.y;
                }
                else
                {
                    bottom = Mathf.Max(bottomYs);
                }

                if (this.top.y != top)
                {
                    result.Add(new BreakLinesResult()
                    {
                        topLeft = new Vector2(this.X, this.top.y),
                        bottomRight = new Vector2(this.X, top),
                        dimention = this.X,
                        isHorizontal = true,
                        camera = null,
                        line = null,
                        main = null,
                    });
                }

                if (this.bottom.y != bottom)
                {
                    result.Add(new BreakLinesResult
                    {
                        topLeft = new Vector2(this.X, bottom),
                        bottomRight = new Vector2(this.X, this.bottom.y),
                        dimention = this.X,
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

        float[] intersectingYs = this.others
            .Where(x => x.left.x <= this.X && x.right.x >= this.X)
            .Select(x => x.Y)
            .Where(x=> x <= this.top.y && x >= this.bottom.y)
            .OrderByDescending(x=>x)
            .ToArray();

        if (this.top.x != intersectingYs[0])
        {
            result.Add(new Segment
            {
                One = new Vector2(this.X, this.top.y),
                Two = new Vector2(this.X, intersectingYs[0]),
                IsY = false
            });
        }

        for (int i = 0; i < intersectingYs.Length - 1; i++)
        {
            result.Add(new Segment
            {
                One = new Vector2(this.X, intersectingYs[i]),
                Two = new Vector2(this.X, intersectingYs[i + 1]),
                IsY = false
            });
        }

        if (this.bottom.x != intersectingYs[intersectingYs.Length - 1])
        {
            result.Add(new Segment
            {
                One = new Vector2(this.X, intersectingYs[intersectingYs.Length - 1]),
                Two = new Vector2(this.X, this.bottom.y),
                IsY = false
            });
        }

        return result.ToArray();
    }
    public void Destroy()
    {
        this.line.Destroy();
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

