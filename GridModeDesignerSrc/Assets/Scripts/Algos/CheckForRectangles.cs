using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckForRectangles
{
    public List<Volume> Check(DelimitorSegmentX[] xs, DelimitorSegmentY[] ys)
    {
        var segmentsSome = new List<Segment>();
        foreach (var item in xs)
        {
            segmentsSome.AddRange(item.Brake());
        }

        foreach (var item in ys)
        {
            segmentsSome.AddRange(item.Brake());
        }

        var segments = new List<Segment>();

        for (int i = 0; i < segmentsSome.Count; i++)
        {
            var seg = segmentsSome[i];
            if (seg.One == seg.Two)
            {
                continue;
            }

            segments.Add(seg);
        }

        var ySegments = segments.Where(x => x.IsY == true).ToArray();

        for (int i = 0; i < ySegments.Length; i++)
        {
            var segment = ySegments[i];
            if (segment.One.x > segment.Two.x)
            {
                var temp = segment.One;
                segment.One = segment.Two;
                segment.Two = temp;
            }
        }

        List<Volume> volumes = new List<Volume>();

        //foreach (var item in ySegments)
        //{
        //this.CheckY(item, item, 0, true, true, segments.ToArray(), new Volume(), volumes);
        //this.CheckY(item, item, 0, false, true, segments.ToArray(), new Volume(), volumes);
        //}

        var item1 = ySegments.OrderByDescending(x => x.One.y).First();
        this.CheckY(item1, item1, 0, true, true, segments.ToArray(), new Volume(), volumes);

        for (int i = 0; i < volumes.Count; i++)
        {
            var x = volumes[i];
            var points = new List<Vector2>();
            foreach (var item in x.Segments)
            {
                points.Add(item.One);
                points.Add(item.Two);
            }

            x.x1 = points.Select(y => y.x).Min();
            x.x2 = points.Select(y => y.x).Max();

            x.y1 = points.Select(y => y.y).Max();
            x.y2 = points.Select(y => y.y).Min();
        }

        return volumes;
    }

    private void CheckY(
        Segment origin,
        Segment seg,
        int turnCount,
        bool goingDown,
        bool goingRight,
        Segment[] allSegments,
        Volume volume,
        List<Volume> results)
    {
        //Debug.Log($"{origin.One.x.ToString("0.00")}-{origin.One.y.ToString("0.00")}|{origin.Two.x.ToString("0.00")}-{origin.Two.y.ToString("0.00")}|| GoingRight {goingRight} GoingDown{goingDown}");
        Color color = Color.red;
        if(turnCount == 0)
        {
            color = Color.green;
            Debug.Log("Green!");
        }
        if(turnCount == 1)
        {
            color = Color.blue;
            Debug.Log("blue!");
        }
        if (turnCount == 2)
        {
            color = Color.cyan;
            Debug.Log("cyan!");
        }
        if (turnCount == 3)
        {
            color = Color.black;
            Debug.Log("black!");
        }

        this.DrawLine(seg.One, seg.Two, color);
        #region SOME
        ///seg and origin are the same the wirst iteration
        seg.Check = true;
        /// origin from -> two
        /// origin to -> one

        var otherXs = (allSegments.Where(x => (x.One == seg.Two || x.Two == seg.Two) && x.IsY == false && x.Check == false).ToArray());
        var otherYs = (allSegments.Where(x => (x.One == seg.Two || x.Two == seg.Two) && x.IsY == true && x.Check == false).ToArray());
        foreach (var item in otherXs)
        {
            if (item.One == seg.Two)
            {
                item.Other = item.Two;
                item.Mathing = item.One;
            }
            else
            {
                item.Other = item.One;
                item.Mathing = item.Two;
            }
        }
        foreach (var item in otherYs)
        {
            if (item.One == seg.Two)
            {
                item.Other = item.Two;
                item.Mathing = item.One;
            }
            else
            {
                item.Other = item.One;
                item.Mathing = item.Two;
            }
        }

        #endregion

        volume.Segments.Add(seg);

        if (seg.Two == origin.One)
        {
            Debug.Log("HugeSuccess!");
            Segment[] destination = new Segment[volume.Segments.Count];
            Array.Copy(volume.Segments.ToArray(), destination, volume.Segments.Count);
            results.Add(new Volume
            {
                Segments = destination.ToList()
            });

            volume.Segments.Remove(seg);
            seg.Check = false;
            return;
        }

        if (turnCount == 4)
        {
            volume.Segments.Remove(seg);
            seg.Check = false;
            Debug.Log("Huge Loss!");
            return;
        }

        //Debug.Log("OTHER X " + otherXs.Length);
        //Debug.Log("OTHER Y " + otherYs.Length);

        foreach (var y in otherYs)
        {
            if ((goingRight && y.Other.x > y.Mathing.x) || (goingRight == false && y.Other.x <= y.Mathing.x))
            {
                CheckY(origin, y, turnCount + 1, !goingDown, !goingRight, allSegments, volume, results);
            }
        }

        foreach (var x in otherXs)
        {
            if ((goingDown && x.Other.y < x.Mathing.y) || (goingDown == false && x.Other.y >= x.Mathing.y))
            {
                CheckY(origin, x, turnCount + 1, !goingDown, !goingRight, allSegments, volume, results);
            }
        }

        volume.Segments.Remove(seg);
        seg.Check = false;
    }

    public Segment[] GetSegments(DelimitorSegmentX[] xs, DelimitorSegmentY[] ys)
    {
        var segments = new List<Segment>();
        foreach (var item in xs)
        {
            segments.AddRange(item.Brake());
        }

        foreach (var item in ys)
        {
            segments.AddRange(item.Brake());
        }

        return segments.ToArray();
    }

    private void DrawX(Vector3 pos, Color color)
    {
        Debug.DrawLine(pos.OffsetY(0.5f), pos.OffsetY(-0.5f), color, 0.3f);
        Debug.DrawLine(pos.OffsetX(0.5f), pos.OffsetX(-0.5f), color, 0.3f);
    }

    private void DrawLine(Vector3 pos1, Vector3 pos2, Color color)
    {
        float offset = 0.02f;
        if (pos1.x == pos2.x)
        {
            Debug.DrawLine(pos1.OffsetX(offset), pos2.OffsetX(offset), color, 2);
            Debug.DrawLine(pos1.OffsetX(-offset), pos2.OffsetX(-offset), color, 2);
        }
        else 
        if(pos1.y == pos2.y)
        {
            Debug.DrawLine(pos1.OffsetY(offset), pos2.OffsetY(offset), color, 2);
            Debug.DrawLine(pos1.OffsetY(-offset), pos2.OffsetY(-offset), color, 2);
        }
        else
        {
            Debug.Log("BIG PP");
        }
    }
}


public class Volume
{
    public List<Segment> Segments { get; set; } = new List<Segment>();

    public float x1 { get; set; }
    public float x2 { get; set; }
    public float y1 { get; set; }
    public float y2 { get; set; }
}

public class Segment
{
    public Vector2 One { get; set; }

    public Vector2 Two { get; set; }

    public Vector2 Other { get; set; }

    public Vector2 Mathing { get; set; }

    public bool IsY { get; set; }

    public bool Check { get; set; } = false;
}
