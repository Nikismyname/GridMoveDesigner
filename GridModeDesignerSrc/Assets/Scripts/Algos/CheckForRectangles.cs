#region INIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CheckForRectangles
{
    private Main main;
    private int debugDelay = 300;
    private bool debug = false;
    private bool test = false;

    public CheckForRectangles(Main main)
    {
        this.main = main;
    }

    public async Task<List<Volume>> Check(DelimitorSegmentX[] xs, DelimitorSegmentY[] ys)
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

        var counter = 0;
        for (int i = 0; i < segmentsSome.Count; i++)
        {
            var seg = segmentsSome[i];
            if (seg.One == seg.Two)
            {
                continue;
            }

            seg.Id = counter++;
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

        //var item1 = ySegments.OrderByDescending(x => x.One.y).Last();
        //await this.CheckY(item1, item1, 0, false, segments.ToArray(), new Volume(), volumes);

        if (test)
        {
            ///###################################SAMPLE TESTS
            bool down = false;
            if (down)
            {
                var top = ySegments.OrderByDescending(x => x.One.y).First(); //TOP
                await this.CheckY(top, top, 0, down, segments.ToArray(), new Volume(), volumes, null);
            }
            else
            {
                var bot = ySegments.OrderByDescending(x => x.One.y).Last(); //BOTTOM
                await this.CheckY(bot, bot, 0, down, segments.ToArray(), new Volume(), volumes, null);
            }
            ///###################################END
        }
        else
        {
            foreach (var ySeg in ySegments)
            {
                await this.CheckY(ySeg, ySeg, 0, true, segments.ToArray(), new Volume(), volumes, Utils.RandColor());
                await this.CheckY(ySeg, ySeg, 0, false, segments.ToArray(), new Volume(), volumes, Utils.RandColor());
            }
        }

        //Debug.Log("###############################################################################");
        foreach (var seg in segments)
        {
            //if (seg.Mathing == Vector2.zero && seg.Other == Vector2.zero)
            //{
            //    Debug.Log($"b {seg.Id}||{seg.One.x.ToString("0.00")} {seg.One.y.ToString("0.00")}|${seg.Two.x.ToString("0.00")} {seg.Two.y.ToString("0.00")} IS_CHECK: {seg.Check}");
            //}
            //else
            //{
            //    Debug.Log($"{seg.Id}||{seg.Mathing.x.ToString("0.00")} {seg.Mathing.y.ToString("0.00")}|${seg.Other.x.ToString("0.00")} {seg.Other.y.ToString("0.00")} IS_CHECK: {seg.Check}");
            //}
            //this.DrawLine(seg.One, seg.Two, Color.black);
        }

        for (int i = 0; i < volumes.Count; i++)
        {
            var vol = volumes[i];
            var points = new List<Vector2>();
            foreach (var item in vol.Segments)
            {
                points.Add(item.One);
                points.Add(item.Two);
            }

            vol.x1 = points.Select(y => y.x).Min();
            vol.x2 = points.Select(y => y.x).Max();

            vol.y1 = points.Select(y => y.y).Max();
            vol.y2 = points.Select(y => y.y).Min();
        }

        return volumes;
    }

    #endregion

    private async Task CheckY(
        Segment origin,
        Segment segment,
        int turnCount,
        bool genDirDown,
        Segment[] allSegments,
        Volume volume,
        List<Volume> results, 
        Color? uniColor
        )
    {
        #region CalcDirections
        //Debug.Log($"ALABALA: {segment.Id} {turnCount}");
        var goingRight = false;
        var goingDown = false;

        if (genDirDown)
        {
            if (turnCount == 0) /// down right 
            {
                goingRight = true;
                goingDown = true;
            }
            if (turnCount == 1) /// down left 
            {
                goingRight = false;
                goingDown = true;
            }
            if (turnCount == 2) /// up left
            {
                goingRight = false;
                goingDown = false;
            }
            if (turnCount == 3) /// up right
            {
                goingRight = true;
                goingDown = false;
            }
            if (turnCount == 4)
            {
                goingRight = true;
                goingDown = false;
            }
        }

        if (genDirDown == false)
        {
            if (turnCount == 0) /// up right
            {
                goingRight = true;
                goingDown = false;
            }
            if (turnCount == 1) /// up left
            {
                goingRight = false;
                goingDown = false;
            }
            if (turnCount == 2) /// down left
            {
                goingRight = false;
                goingDown = true;
            }
            if (turnCount == 3) /// down right
            {
                goingRight = true;
                goingDown = true;
            }
            if (turnCount == 4)
            {
                goingRight = true;
                goingDown = false;
            }
        }

        #endregion

        #region DEBUG_VISUAL

        if (this.debug)
        {

            Color color = Color.clear;
            if (volume.Segments.Count == 0)
            {
                color = Color.cyan;
            }
            else if (volume.Segments.Last() == segment)
            {
                color = Color.green;
            }
            else if (segment.One.y == segment.Two.y)
            {
                color = Color.red;
            }
            else if (segment.Matching.y > segment.Other.y) /// DOWN
            {
                color = Color.black;
            }
            else
            {
                color = Color.white; /// UP
            }
            if (uniColor != null)
            {
                Utils.DrawThickLine(segment.One, segment.Two, uniColor.Value, 0.5f);
            }
            else
            {
                Utils.DrawThickLine(segment.One, segment.Two, color, 0.5f);
            }

            this.main.ID = segment.Id.ToString();
            await Task.Delay(this.debugDelay);
        }

        #endregion

        #region DEBUG TEXT

        //Color color = Color.red;
        //if (turnCount == 0)
        //{
        //    color = Color.green;
        //    if (segment.Matching == Vector2.zero && segment.Other == Vector2.zero)
        //    {
        //        Debug.Log("Green! b " + $"{segment.Id}||{segment.One.x.ToString("0.00")} {segment.One.y.ToString("0.00")}|${segment.Two.x.ToString("0.00")} {segment.Two.y.ToString("0.00")}");
        //    }
        //    else
        //    {
        //        Debug.Log("Green!" + $"{segment.Id}||{segment.Matching.x.ToString("0.00")} {segment.Matching.y.ToString("0.00")}|${segment.Other.x.ToString("0.00")} {segment.Other.y.ToString("0.00")}");
        //    }
        //}
        //if (turnCount == 1)
        //{
        //    color = Color.blue;
        //    if (segment.Matching == Vector2.zero && segment.Other == Vector2.zero)
        //    {
        //        Debug.Log("blue! b " + $"{segment.Id}||{segment.One.x.ToString("0.00")} {segment.One.y.ToString("0.00")}|${segment.Two.x.ToString("0.00")} {segment.Two.y.ToString("0.00")}");
        //    }
        //    else
        //    {
        //        Debug.Log("blue!" + $"{segment.Id}||{segment.Matching.x.ToString("0.00")} {segment.Matching.y.ToString("0.00")}|${segment.Other.x.ToString("0.00")} {segment.Other.y.ToString("0.00")}");
        //    }
        //}
        //if (turnCount == 2)
        //{
        //    color = Color.cyan;
        //    if (segment.Matching == Vector2.zero && segment.Other == Vector2.zero)
        //    {
        //        Debug.Log("cyan! b " + $"{segment.Id}||{segment.One.x.ToString("0.00")} {segment.One.y.ToString("0.00")}|${segment.Two.x.ToString("0.00")} {segment.Two.y.ToString("0.00")}");
        //    }
        //    else
        //    {
        //        Debug.Log("cyan!" + $"{segment.Id}||{segment.Matching.x.ToString("0.00")} {segment.Matching.y.ToString("0.00")}|${segment.Other.x.ToString("0.00")} {segment.Other.y.ToString("0.00")}");
        //    }
        //}
        //if (turnCount == 3)
        //{
        //    color = Color.black;
        //    if (segment.Matching == Vector2.zero && segment.Other == Vector2.zero)
        //    {
        //        Debug.Log("black! b " + $"{segment.Id}||{segment.One.x.ToString("0.00")} {segment.One.y.ToString("0.00")}|${segment.Two.x.ToString("0.00")} {segment.Two.y.ToString("0.00")}");
        //    }
        //    else
        //    {
        //        Debug.Log("black!" + $"{segment.Id}||{segment.Matching.x.ToString("0.00")} {segment.Matching.y.ToString("0.00")}|${segment.Other.x.ToString("0.00")} {segment.Other.y.ToString("0.00")}");
        //    }
        //}

        //Debug.Log("#################################################################################################################");

        //foreach (var seg1 in allSegments)
        //{
        //    if (segment.Matching == Vector2.zero || segment.Other == Vector2.zero)
        //    {
        //        Debug.Log($"b {seg1.Id}||{seg1.One.x.ToString("0.00")} {seg1.One.y.ToString("0.00")}|${seg1.Two.x.ToString("0.00")} {seg1.Two.y.ToString("0.00")} IS_CHECK: {seg1.Check}");
        //    }
        //    else
        //    {
        //        Debug.Log($"{seg1.Id}||{seg1.Matching.x.ToString("0.00")} {seg1.Matching.y.ToString("0.00")}|${seg1.Other.x.ToString("0.00")} {seg1.Other.y.ToString("0.00")} IS_CHECK: {seg1.Check}");
        //    }
        //}

        //Debug.Log("#################################################################################################################");

        /////seg and origin are the same the wirst iteration
        //segment.Check = true;
        ///// origin from -> two
        ///// origin to -> one

        #endregion

        if (segment == origin)
        {
            segment.Other = segment.Two;
            segment.Matching = segment.One;
        }

        #region GET_NEXT_POSITIONS

        List<Segment> filteredXs = new List<Segment>();
        List<Segment> otherXs = new List<Segment>();
        if (turnCount < 4)
        {
            foreach (var x in allSegments)
            {
                if ((x.One == segment.Other || x.Two == segment.Other) && x.Id != segment.Id && x.IsY == false && x.Check == false)
                {
                    otherXs.Add(new Segment
                    {
                        Id = x.Id,
                        Check = x.Check,
                        IsY = x.IsY,
                        Matching = x.Matching,
                        Other = x.Other,
                        One = x.One,
                        Two = x.Two,
                    });
                }
            }
            foreach (var item in otherXs)
            {
                if (item.One == segment.Other)
                {
                    item.Other = item.Two;
                    item.Matching = item.One;
                }
                else
                {
                    item.Other = item.One;
                    item.Matching = item.Two;
                }
            }
            foreach (var x in otherXs)
            {
                if ((goingDown && x.Other.y < x.Matching.y) || (goingDown == false && x.Other.y >= x.Matching.y))
                {
                    filteredXs.Add(x);
                }
            }
        }

        List<Segment> filteredYs = new List<Segment>();
        List<Segment> otherYs = new List<Segment>();
        foreach (var x in allSegments)
        {
            if ((x.One == segment.Other || x.Two == segment.Other) && x.Id != segment.Id && x.IsY == true && x.Check == false)
            {
                otherYs.Add(new Segment
                {
                    Id = x.Id,
                    Check = x.Check,
                    IsY = x.IsY,
                    Matching = x.Matching,
                    Other = x.Other,
                    One = x.One,
                    Two = x.Two,
                });
            }
        }
        foreach (var item in otherYs)
        {
            if (item.One == segment.Other)
            {
                item.Other = item.Two;
                item.Matching = item.One;
            }
            else
            {
                item.Other = item.One;
                item.Matching = item.Two;
            }
        }
        foreach (var y in otherYs)
        {
            if ((goingRight && y.Other.x > y.Matching.x) || (goingRight == false && y.Other.x <= y.Matching.x))
            {
                filteredYs.Add(y);
            }
        }

        #endregion

        volume.Segments.Add(segment);
       
        if (segment.Other == origin.One && segment.Id != origin.Id)
        {
            //int segId = segment.Id;
            //if (results.Any(x => x.Segments.Select(y => y.Id).OrderBy(y => y).SequenceEqual(volume.Segments.Select(y => y.Id).OrderBy(y => y))))
            //{
            //    //Debug.Log("Already got this mate!");
            //    return;
            //}

            var exst = results.Select(x=>x.Segments.Select(y=>y.Id).OrderBy(y=>y).ToArray());
            var curr = volume.Segments.Select(x => x.Id).OrderBy(x => x).ToArray();

            //Debug.Log("Mine" + string.Join(", ", curr));
            foreach (var item in exst)
            {
                //Debug.Log("Other" + string.Join(", ", item));
            }

            if (exst.Any(x => x.SequenceEqual(curr)))
            {
                //Debug.Log("DOUBLE!");
                volume.Segments.Remove(segment);
                segment.Check = false;
                return;
            }

            Segment[] destination = new Segment[volume.Segments.Count];
            Array.Copy(volume.Segments.ToArray(), destination, volume.Segments.Count);
            results.Add(new Volume
            {
                Segments = destination.ToList()
            });

            volume.Segments.Remove(segment);
            segment.Check = false;
            return;
        }

        int newYTurnCount = turnCount;
        if (turnCount == 1 || turnCount == 3)
        {
            newYTurnCount += 1;
        }
        foreach (var y in filteredYs)
        {
            await CheckY(origin, y, newYTurnCount, genDirDown, allSegments, volume, results, uniColor);
        }

        int newXTurnCount = turnCount;
        if (turnCount == 0 || turnCount == 2)
        {
            newXTurnCount += 1;
        }
        foreach (var x in filteredXs)
        {
            await CheckY(origin, x, newXTurnCount, genDirDown, allSegments, volume, results, uniColor);
        }

        volume.Segments.Remove(segment);
        segment.Check = false;
        return;
    }

    #region OTHERS

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

    public bool RectIntLine(
        float x1,
        float x2,
        float y1,
        float y2,
        Vector2 line1,
        Vector2 line2,
        bool inclusive = true)
    {
        if (line1.x == line2.x) // |
        {
            Vector2 top;
            Vector2 bot;

            if (line1.y < line2.y)
            {
                top = line2;
                bot = line1;
            }
            else
            {
                top = line2;
                bot = line1;
            }

            if (line1.x >= x1 && line1.x <= x2)
            {
                if (bot.y > y1)
                {
                    return false;
                }

                if (top.y < y2)
                {
                    return false;
                }

                return true;
            }
        }
        else if (line1.y == line2.y) // -
        {
            Vector2 left;
            Vector2 right;

            if (line1.x < line2.x)
            {
                left = line1;
                right = line2;
            }
            else
            {
                right = line1;
                left = line2;
            }

            if (line1.y <= y1 && line1.y >= y2)
            {
                if (left.x < x1)
                {
                    return false;
                }

                if (right.x > x2)
                {
                    return false;
                }

                return true;
            }
        }

        return false;
    }

    public bool PointInsideRect(Vector2 rectTopLeft, float rectLength, float rectHeight, Vector2 point, bool inclusive = false)
    {
        if (inclusive)
        {
            if (point.x <= rectTopLeft.x + rectLength && point.x >= rectTopLeft.x
                && point.y >= rectTopLeft.y - rectHeight && point.y <= rectTopLeft.y)
            {
                return true;
            }

            return false;
        }
        else
        {
            if (point.x < rectTopLeft.x + rectLength && point.x > rectTopLeft.x
                && point.y > rectTopLeft.y - rectHeight && point.y < rectTopLeft.y)
            {
                return true;
            }

            return false;
        }
    }

    public bool DoRectsOverlap(Vector2 l1, Vector2 r1, Vector2 l2, Vector2 r2, bool inclusive/*, int id1, int id2*/)
    {
        if (inclusive)
        {
            // If one rectangle is on left side of other  
            if (l1.x >= r2.x || l2.x >= r1.x)
            {
                return false;
            }

            // If one rectangle is above other  
            if (l1.y <= r2.y || l2.y <= r1.y)
            {
                return false;
            }
            return true;
        }
        else
        {
            // If one rectangle is on left side of other  
            if (l1.x > r2.x || l2.x > r1.x)
            {
                // Debug.LogError(); 
                return false;
            }

            // If one rectangle is above other  
            if (l1.y < r2.y || l2.y < r1.y)
            {
                return false;
            }
            return true;
        }
    }

    #endregion
}


#region MODELS

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

    public Vector2 Matching { get; set; }

    public bool IsY { get; set; }

    public bool Check { get; set; } = false;

    public int Id { get; set; }
}


//Color color = Utils.RandColor(); 
//Utils.DrawEdgeLine(segment.Mathing, segment.Other, color);

#endregion