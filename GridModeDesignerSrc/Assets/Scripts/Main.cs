using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    public GameObject CylinderPrefab;
    private List<DelimitorSegmentX> Xsegments = new List<DelimitorSegmentX>();
    private List<DelimitorSegmentY> Ysegments = new List<DelimitorSegmentY>();
    private Camera camera;

    private bool linesHidden = false;

    public float xLeft;
    public float xRight;
    public float xWidth;
    public float yTop;
    public float yBottom;
    public float yHeight;

    public EditingModes mode;

    private CheckForRectangles rectCheck = new CheckForRectangles();

    void Start()
    {
        this.mode = EditingModes.placingLines;
        this.camera = Camera.main;
        var btn = GameObject.Find("SetBtn");
        this.GetScreenDimentions();

        this.CreateLineX(new Vector2(xLeft, yBottom), new Vector2(xLeft, yTop));
        this.CreateLineX(new Vector2(xRight, yBottom), new Vector2(xRight, yTop));
        this.CreateLineY(new Vector2(xLeft ,yBottom), new Vector2(xRight, yBottom));
        this.CreateLineY(new Vector2(xLeft, yTop), new Vector2(xRight, yTop)); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            this.mode = EditingModes.breakingLines;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.mode = EditingModes.placingLines;
        }

        ///TODO: Implement undo!
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            //this.segments[this.segments.Count - 1].remove(); 
        }

        if (this.mode == EditingModes.placingLines)
        {
            this.PlaceLines();
        }
        else
        {
            this.BreakLines();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (this.linesHidden == false)
            {
                //var things = this.rectCheck.GetSegments(this.Xsegments.ToArray(), this.Ysegments.ToArray());
                //foreach (var thing in things)
                //{
                //    Color background = new Color(
                //      UnityEngine.Random.Range(0f, 1f),
                //      UnityEngine.Random.Range(0f, 1f),
                //      UnityEngine.Random.Range(0f, 1f)
                //  );

                //    this.DrawLine(thing.One, thing.Two, background);
                //}

                Debug.Log("T");
                var result = this.rectCheck.Check(this.Xsegments.ToArray(), this.Ysegments.ToArray());
                if(result.Count == 0)
                {
                    Debug.Log("UNACCEPTABBLE CONDITION");
                }
                foreach (var item in result)
                {
                    Debug.Log($"{item.x1} {item.x2} {item.y1} {item.y2}");

                    Debug.Log("HERE " +item.Segments.Count);
                    //foreach (var segment in item.Segments)
                    //{
                    //    Debug.Log(se);
                    //}

                    Color background = new Color(
                      UnityEngine.Random.Range(0f, 1f),
                      UnityEngine.Random.Range(0f, 1f),
                      UnityEngine.Random.Range(0f, 1f)
                    );

                    var one = new Vector2(item.x1, item.y1);
                    var two = new Vector2(item.x2, item.y1);
                    var three = new Vector2(item.x2, item.y2);
                    var four = new Vector2(item.x1, item.y2);

                    this.DrawLine(one, two, background);
                    this.DrawLine(two, three, background);
                    this.DrawLine(three, four, background);
                    this.DrawLine(four, one, background);
                }

                this.HideAllLines();
                this.linesHidden = true;
            }
            else
            {
                this.linesHidden = false;
                this.ShowAllLines();
            }
        }
    }

    private void BreakLines()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mp = Input.mousePosition;
            Vector3 pos = this.camera.GetComponent<Camera>().ScreenToWorldPoint(mp).OffsetZ(11);

            List<DelimitorSegmentX> xes = new List<DelimitorSegmentX>();
            List<BreakLinesResult> breakLinesX = new List<BreakLinesResult>();
            for (int i = 0; i < this.Xsegments.Count; i++)
            {
                var item = this.Xsegments[i];
                var res = item.BreakLine(pos);
                if (res != null)
                {
                    xes.Add(item);
                    breakLinesX.AddRange(res);
                }
            }
            for (int i = xes.Count - 1; i >= 0; i--)
            {
                this.Xsegments.Remove(xes[i]);
                xes[i].Destroy();
            }
            this.BreakLinesX(breakLinesX.ToArray());

            List<DelimitorSegmentY> ys = new List<DelimitorSegmentY>();
            List<BreakLinesResult> breakLinesY = new List<BreakLinesResult>();
            for (int i = 0; i < this.Ysegments.Count; i++)
            {
                var item = this.Ysegments[i];
                var res = item.BreakLine(pos);
                if (res != null)
                {
                    ys.Add(item);
                    breakLinesY.AddRange(res);
                }
            }
            for (int i = 0; i < ys.Count; i++)
            {
                this.Ysegments.Remove(ys[i]);
                ys[i].Destroy();
            }
            this.BreakLinesY(breakLinesY.ToArray());

            this.mode = EditingModes.placingLines;
        }
    }

    private void BreakLinesX(BreakLinesResult[] results)
    {
        foreach (var x in results)
        {
            this.CreateLineX(x.bottomRight, x.topLeft);
        }
    }

    private void BreakLinesY(BreakLinesResult[] results)
    {
        foreach (var x in results)
        {
            this.CreateLineY(x.bottomRight, x.topLeft);
        }
    }

    private void PlaceLines()
    {
        bool isY = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isY = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mp = Input.mousePosition;
            Vector3 pos = this.camera.GetComponent<Camera>().ScreenToWorldPoint(mp).OffsetZ(11);
            //Debug.Log($"{pos.x.ToString("0.00")} {pos.y.ToString("0.00")}");

            if (this.Xsegments.Any(x => x.IsColliding(pos)) || this.Ysegments.Any(x => x.IsColliding(pos)))
            {
                //Debug.Log("OVER");
                return;
            }
            else
            {
                //Debug.Log("NOT OVER!");
            }

            if (isY)
            {
                this.CreateLineYPlacing(new Vector3(this.xLeft, pos.y, pos.z), new Vector3(this.xRight, pos.y, pos.z));
            }
            else
            {
                this.CreateLineXPlacing(new Vector3(pos.x, this.yTop, pos.z), new Vector3(pos.x, this.yBottom, pos.z));
            }
        }
    }

    private void CreateLineXPlacing(Vector3 pos, Vector3 pos2)
    {
        var line = new Line();
        line.SetUp(pos, pos2, Color.red, new GameObject().transform, "some", 0.1f, this.CylinderPrefab);
        var seg = line.line.AddComponent<DelimitorSegmentX>();
        seg.SetUp(line, this.Ysegments, this.camera, pos.x, new Vector2(pos.x, this.yTop), new Vector2(pos.x, this.yBottom), this);
        this.Xsegments.Add(seg);
    }

    private void CreateLineYPlacing(Vector3 pos, Vector3 pos2)
    {
        var line = new Line();
        line.SetUp(pos, pos2, Color.red, new GameObject().transform, "some", 0.1f, this.CylinderPrefab);
        var seg = line.line.AddComponent<DelimitorSegmentY>();
        seg.SetUp(line, this.Xsegments, this.camera, pos.y, new Vector2(this.xLeft, pos.y), new Vector2(this.xRight, pos.y), this);
        this.Ysegments.Add(seg);
    }

    private void CreateLineX(Vector3 pos, Vector3 pos2)
    {
        var line = new Line();
        line.SetUp(pos, pos2, Color.red, new GameObject().transform, "some", 0.1f, this.CylinderPrefab);
        var seg = line.line.AddComponent<DelimitorSegmentX>();
        seg.SetUp(line, this.Ysegments, this.camera, pos.x, pos, pos2, this);
        this.Xsegments.Add(seg);
    }

    private void CreateLineY(Vector3 pos, Vector3 pos2)
    {
        var line = new Line();
        line.SetUp(pos, pos2, Color.red, new GameObject().transform, "some", 0.1f, this.CylinderPrefab);
        var seg = line.line.AddComponent<DelimitorSegmentY>();
        seg.SetUp(line, this.Xsegments, this.camera, pos.y, pos, pos2, this);
        this.Ysegments.Add(seg);
    }

    #region HELPERS

    private void GetScreenDimentions()
    {
        Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

        //Debug.Log("BOTTOM_LEFT: " + bottomLeft);
        //Debug.Log("TOP_RIGHT: " + topRight);

        this.xLeft = bottomLeft.x;
        this.xRight = topRight.x;
        this.xWidth = Math.Abs(this.xLeft - this.xRight);
        this.yTop = topRight.y;
        this.yBottom = bottomLeft.y;
        this.yHeight = Math.Abs(this.yTop - this.yBottom);
    }

    private void DrawX(Vector3 pos, Color color)
    {
        Debug.DrawLine(pos.OffsetY(0.5f), pos.OffsetY(-0.5f), color, 0.3f);
        Debug.DrawLine(pos.OffsetX(0.5f), pos.OffsetX(-0.5f), color, 0.3f);
    }

    private void DrawLine(Vector3 pos1, Vector3 pos2, Color color)
    {
        Debug.DrawLine(pos1, pos2, color, 20);
    }

    public void HideAllLines()
    {
        foreach (var segment in this.Xsegments)
        {
            segment.Hide();
        }

        foreach (var segment in this.Ysegments)
        {
            segment.Hide();
        }
    }

    public void ShowAllLines()
    {
        foreach (var segment in this.Xsegments)
        {
            segment.Show();
        }

        foreach (var segment in this.Ysegments)
        {
            segment.Show();
        }
    }

    #endregion
}
