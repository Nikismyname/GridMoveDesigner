using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

    public Volume[] ConvertToVolumes(string text)
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

        return result.ToArray();
    }
}

