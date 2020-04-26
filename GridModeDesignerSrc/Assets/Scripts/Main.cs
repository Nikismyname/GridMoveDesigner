using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public string ID;

    public EditingModes mode;

    private CheckForRectangles rectCheck;

    public GameObject textMeshPrefab;

    void Start()
    {
        this.rectCheck = new CheckForRectangles(this);
        this.mode = EditingModes.placingLines;
        this.camera = Camera.main;
        var btn = GameObject.Find("SetBtn");
        this.GetScreenDimentions();

        this.CreateLineX(new Vector2(xLeft, yBottom), new Vector2(xLeft, yTop));
        this.CreateLineX(new Vector2(xRight, yBottom), new Vector2(xRight, yTop));
        this.CreateLineY(new Vector2(xLeft, yBottom), new Vector2(xRight, yBottom));
        this.CreateLineY(new Vector2(xLeft, yTop), new Vector2(xRight, yTop));
    }

    // Update is called once per frame
    async void Update()
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
                this.HideAllLines();
                List<Volume> result = await this.rectCheck.Check(this.Xsegments.ToArray(), this.Ysegments.ToArray());
                if (result.Count == 0)
                {
                    Debug.Log("UNACCEPTABBLE CONDITION");
                }
                else
                {
                    Debug.Log($"COUNT ONE: {result.Count}");

                    List<Volume> blackList = new List<Volume>();

                    result = result
                        .OrderByDescending(x => Math.Abs(x.x1 - x.x2))
                        .ThenByDescending(x => Math.Abs(x.y1 - x.y2))
                        .ToList();

                    for (int i = 0; i < result.Count; i++)
                    {
                        Volume curr = result[i];
                        for (int j = i + 1; j < result.Count; j++)
                        {
                            Volume other = result[j];

                            if (this.rectCheck.DoRectsOverlap(new Vector2(curr.x1, curr.y1), new Vector2(curr.x2, curr.y2),
                                new Vector2(other.x1, other.y1), new Vector2(other.x2, other.y2), true) == true)
                            {
                                var currWidt = Math.Abs(curr.x1 - curr.x2);
                                var currHeight = Math.Abs(curr.y1 - curr.y2);
                                var otherWidt = Math.Abs(other.x1 - other.x2);
                                var otherHeight = Math.Abs(other.y1 - other.y2);

                                if (currWidt <= otherWidt && currHeight <= otherHeight)
                                {
                                    blackList.Add(other);
                                }
                                else if (currWidt >= otherWidt && currHeight >= otherHeight)
                                {
                                    blackList.Add(curr);
                                }
                                else
                                {
                                    //Debug.Log("HERE");
                                }
                            }
                        }
                    }

                    result = result.Where(x => blackList.Contains(x) == false).ToList();

                    Debug.Log($"COUNT TWO: {result.Count}");
                }

                this.DrawResultsWithDelay(result, false, 500);

                this.linesHidden = true;
            }
            else
            {
                this.linesHidden = false;
                this.ShowAllLines();
            }
        }
    }

    private async void DrawResultsWithDelay(List<Volume> result, bool wait, int ms = 0)
    {
        float time = 2;
        foreach (var item in result)
        {
            //Color background = new Color(
            //  UnityEngine.Random.Range(0f, 1f),
            //  UnityEngine.Random.Range(0f, 1f),
            //  UnityEngine.Random.Range(0f, 1f)
            //);

            //var one = new Vector2(item.x1, item.y1);
            //var two = new Vector2(item.x2, item.y1);
            //var three = new Vector2(item.x2, item.y2);
            //var four = new Vector2(item.x1, item.y2);

            //Utils.DrawThickLine(one, two, background, time);
            //Utils.DrawThickLine(two, three, background, time);
            //Utils.DrawThickLine(three, four, background, time);
            //Utils.DrawThickLine(four, one, background, time);

            if (item.Segments.Select(x => x.Id).Distinct().Count() != item.Segments.Select(x => x.Id).Count())
            {
                Debug.LogError("ITEM SENDT TWICE");
            }

            //Debug.Log(string.Join(" ", item.Segments.Select(x => x.Id)/*.OrderBy(x => x)*/) + $" W:{item.x2 - item.x1} H:{item.y1 - item.y2}");

            foreach (var seg in item.Segments)
            {
                Color color = new Color(
                  UnityEngine.Random.Range(0f, 1f),
                  UnityEngine.Random.Range(0f, 1f),
                  UnityEngine.Random.Range(0f, 1f)
                );
                Utils.DrawThickLine(seg.One, seg.Two, color, time);

                //var meshGo = GameObject.Instantiate(this.textMeshPrefab);
                //TextMesh tmesh = meshGo.GetComponent<TextMesh>();
                //var pos = Vector3.Lerp(seg.One, seg.Two, 0.5f);
                //meshGo.transform.position = Vector3.Lerp(pos, new Vector3(0,0, pos.z), 0.2f);
                //tmesh.text = seg.Id.ToString();
            }

            if (wait)
            {
                await Task.Delay(ms);
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

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 25;
        GUI.Label(new Rect(new Vector2(0, 0), new Vector2(200, 200)), this.ID);
    }
}
