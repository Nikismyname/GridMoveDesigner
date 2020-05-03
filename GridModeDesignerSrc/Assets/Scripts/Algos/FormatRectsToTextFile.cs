namespace SerDes
{
    using RecRec;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class FormatRectsToTextFile
    {
        private Main main;

        public FormatRectsToTextFile(Main main)
        {
            this.main = main;
        }

        public string ConvertToText(Volume[] volumes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Groups]");
            sb.AppendLine($"NumberOfGroups = {volumes.Length}");
            int counter = 1;

            foreach (var item in volumes)
            {
                float x1 = Math.Abs(this.main.xLeft - item.x1) / this.main.xWidth;
                float x2 = Math.Abs(this.main.xLeft - item.x2) / this.main.xWidth;
                float y1 = Math.Abs(this.main.yTop - item.y1) / this.main.yHeight;
                float y2 = Math.Abs(this.main.yTop - item.y2) / this.main.yHeight;

                sb.AppendLine($"[{counter++}]");
                sb.AppendLine($"TriggerTop   = [Monitor1Top] +  [Monitor1Height] * {y1}");
                sb.AppendLine($"TriggerBottom= [Monitor1Top] +  [Monitor1Height] * {y2}");
                sb.AppendLine($"TriggerLeft  = [Monitor1Left] + [Monitor1Width] * {x1}");
                sb.AppendLine($"TriggerRight = [Monitor1Left] + [Monitor1Width] * {x2 }");
            }

            return sb.ToString();
        }

        private Segment[] GetSegmentsFromVolume(Volume[] volumes)
        {
            var result = new List<Segment>();

            foreach (var item in volumes)
            {
                Vector2 x1y1 = new Vector2(item.x1, item.y1);
                Vector2 x1y2 = new Vector2(item.x1, item.y2);
                Vector2 x2y1 = new Vector2(item.x2, item.y1);
                Vector2 x2y2 = new Vector2(item.x2, item.y2);

                result.Add(new Segment(x1y1, x2y1));
                result.Add(new Segment(x1y2, x2y2));
                result.Add(new Segment(x1y1, x1y2));
                result.Add(new Segment(x2y1, x2y2));
            }

            return result.ToArray();
        }

        public Segment[] ConvertToVolumes(string text)
        {
            List<Volume> result = new List<Volume>();

            string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            lines = lines.Select(x => x.Trim()).ToArray();

            string boxNumRegex = "^[[0-9]+]$";
            string topRegex = @"^TriggerTop.+\*\s*([0-9]+(?:\.[0-9]+)?)$";
            string botRegex = @"^TriggerBottom.+\*\s*([0-9]+(?:\.[0-9]+)?)$";
            string leftRegex = @"^TriggerLeft.+\*\s*([0-9]+(?:\.[0-9]+)?)$";
            string rightRegex = @"^TriggerRight.+\*\s*([0-9]+(?:\.[0-9]+)?)$";

            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i], boxNumRegex))
                {
                    string topMatch = Regex.Match(lines[i + 1], topRegex).Groups[1].Value;
                    string botMatch = Regex.Match(lines[i + 2], botRegex).Groups[1].Value;
                    string leftMatch = Regex.Match(lines[i + 3], leftRegex).Groups[1].Value;
                    string rightMatch = Regex.Match(lines[i + 4], rightRegex).Groups[1].Value;

                    float topRatio = float.Parse(topMatch);
                    float botRatio = float.Parse(botMatch);
                    float leftRatio = float.Parse(leftMatch);
                    float rightRatio = float.Parse(rightMatch);

                    result.Add(new Volume
                    {
                        y1 = this.main.yTop - topRatio * this.main.yHeight,
                        y2 = this.main.yTop - botRatio * this.main.yHeight,
                        x1 = this.main.xLeft + leftRatio * this.main.xWidth,
                        x2 = this.main.xLeft + rightRatio * this.main.xWidth,
                    });
                }
            }

            var segments = this.CombineSegments(result.ToArray());
            //var segments = this.GetSegmentsFromVolume(result.ToArray());

            return segments;
        }

        public Segment[] CombineSegments(Volume[] vInput)
        {
            List<Segment> input = this.GetSegmentsFromVolume(vInput).ToList();

            List<Segment> result = new List<Segment>();
            List<Segment> xx = new List<Segment>();
            List<Segment> yy = new List<Segment>();

            foreach (var item in input)
            {
                if (item.x1 == item.x2)
                {
                    yy.Add(item);
                }
                else if (item.y1 == item.y2)
                {
                    xx.Add(item);
                }
            }

            yy = yy.OrderBy(x => x.x2).ThenBy(x => x.y1).ToList();
            xx = xx.OrderBy(x => x.y2).ThenBy(x => x.x1).ToList();

            //List<Volume> finalY = new List<Volume>();
            //List<Volume> finalX = new List<Volume>();

            List<Segment> temp = new List<Segment>();
            if (xx.Count > 0)
            {
                temp.Add(xx[0]);
                for (int i = 1; i < xx.Count; i++)
                {
                    var prev = xx[i - 1];
                    var curr = xx[i];
                    if (prev.y2 == curr.y1 || prev.y1 == curr.y2)
                    {
                        temp.Add(curr);
                    }
                    else
                    {
                        result.Add(this.CombineVolumeArr(temp.ToArray()));
                        temp.Clear();
                        temp.Add(curr);
                    }
                }
                if (temp.Count != 0)
                {
                    result.Add(this.CombineVolumeArr(temp.ToArray()));
                    temp.Clear();
                }
            }

            if (yy.Count > 0)
            {
                temp.Add(yy[0]);
                for (int i = 1; i < yy.Count; i++)
                {
                    var prev = yy[i - 1];
                    var curr = yy[i];
                    if (prev.x2 == curr.x1 || prev.x1 == curr.x2)
                    {
                        temp.Add(curr);
                    }
                    else
                    {
                        result.Add(this.CombineVolumeArr(temp.ToArray()));
                        temp.Clear();
                        temp.Add(curr);
                    }
                }
                if (temp.Count != 0)
                {
                    result.Add(this.CombineVolumeArr(temp.ToArray()));
                    temp.Clear();
                }
            }

            for (int i = 0; i < result.Count; i++)
            {
                result[i].PopulateVectors();
            }

            return result.ToArray();
        }

        private Segment CombineVolumeArr(Segment[] volumes)
        {
            return new Segment()
            {
                x1 = volumes.Select(x => x.x1).Min(),
                x2 = volumes.Select(x => x.x2).Max(),
                y1 = volumes.Select(x => x.y1).Min(),
                y2 = volumes.Select(x => x.y2).Max(),
            };
        }
    }

    public class Segment
    {
        public Segment() { }

        public Segment(Vector2 TL, Vector2 BR)
        {
            this.TL = TL;
            this.BR = BR; 

            if (TL.x == BR.x)
            {
                if (TL.y > BR.y)
                {
                    var temp = TL;
                    TL = BR;
                    BR = temp;
                }
            }
            else if (TL.y == BR.y)
            {
                if (TL.x > BR.x)
                {
                    var temp = TL;
                    TL = BR;
                    BR = temp;
                }
            }
            else
            {
                throw new Exception("SOME EXC");
            }

            this.x1 = TL.x;
            this.x2 = BR.x;
            this.y1 = TL.y;
            this.y2 = BR.y;
        }

        public float x1 { get; set; }

        public float x2 { get; set; }

        public float y1 { get; set; }

        public float y2 { get; set; }

        public Vector2 TL { get; set; }

        public Vector2 BR { get; set; }

        public void PopulateVectors()
        {
            var tlX = Math.Min(x1, x2);
            var tlY = Math.Max(y1, y2);
            var brX = Math.Max(x1, x2);
            var brY = Math.Min(y1, y2);

            this.TL = new Vector2(tlX, tlY);
            this.BR = new Vector2(brX, brY);
        }
    }
}