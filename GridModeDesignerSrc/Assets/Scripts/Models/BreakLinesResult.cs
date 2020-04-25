using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

public class BreakLinesResult
{
    public Line line { get; set; }
    public List<DelimitorSegmentX> othersX { get; set; }
    public List<DelimitorSegmentY> othersY { get; set; }
    public Camera camera { get; set; }
    public bool isHorizontal { get; set; }
    public float dimention { get; set; }
    public Vector2 topLeft { get; set; }
    public Vector2 bottomRight { get; set; }
    public Main main { get; set; }
}

