﻿using Assets.Scripts.Helpers;
using RecRec;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using ser = SerDes;

public class Main : MonoBehaviour
{
    [SerializeField]
    public GameObject CylinderPrefab;
    private List<DelimitorSegmentX> Xsegments = new List<DelimitorSegmentX>();
    private List<DelimitorSegmentY> Ysegments = new List<DelimitorSegmentY>();
    private Camera myCamera;

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
    private ser.FormatRectsToTextFile toText;

    public GameObject textMeshPrefab;
    private UiManager ui;
    private DataManager data;

    private void Awake()
    {
        this.data = new DataManager();
        this.data.InitializeFile();
    }

    void Start()
    {
        this.rectCheck = new CheckForRectangles(this);
        this.toText = new ser.FormatRectsToTextFile(this);
        this.myCamera = Camera.main;
        this.mode = EditingModes.menu;
        this.ui = gameObject.GetComponent<UiManager>();
        this.GetScreenDimentions();
        this.AddBorderLines();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (this.mode != EditingModes.menu)
            {
                this.mode = EditingModes.menu;
                this.ui.OnEnableMenu();
            } 
        }

        if (this.mode == EditingModes.menu)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            this.MyReset();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            this.NewSave();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            this.NewLoad();
        }

        if (Input.GetKeyDown(KeyCode.B) && this.mode != EditingModes.menu)
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
            //if (this.linesHidden == false)
            //{
            //    //var result = await this.GenerateVolumes();
            //    //this.DrawResultsWithDelay(result.ToList(), false, 500);

            //    this.linesHidden = true;
            //    this.HideAllLines();
            //}
            //else
            //{
            //    this.linesHidden = false;
            //    this.ShowAllLines();
            //}

            Debug.LogError("some error");
            Debug.Log("some log");
            Debug.LogWarning("some warning");
        }
    }

    private void DrawVolumesSegments(ser.Segment[] segments)
    {
        foreach (var segment in segments)
        {
            if (segment.x1 == segment.x2)
            {
                this.CreateLineX(segment.TL, segment.BR);
            }
            else
            {
                this.CreateLineY(segment.TL, segment.BR);
            }
        }
    }

    private void OldDrawVolumesSegments(Volume[] volums)
    {
        foreach (var item in volums)
        {
            Vector2 x1y1 = new Vector2(item.x1, item.y1);
            Vector2 x1y2 = new Vector2(item.x1, item.y2);
            Vector2 x2y1 = new Vector2(item.x2, item.y1);
            Vector2 x2y2 = new Vector2(item.x2, item.y2);

            this.CreateLineY(x1y1, x2y1);
            this.CreateLineY(x1y2, x2y2);
            this.CreateLineX(x1y1, x1y2);
            this.CreateLineX(x2y1, x2y2);
        }
    }

    private async Task<Volume[]> GenerateVolumes(bool filterOverlap = true)
    {
        List<Volume> result = await this.rectCheck.Check(this.Xsegments.ToArray(), this.Ysegments.ToArray());
        result = result
            .OrderByDescending(x => Math.Abs(x.x1 - x.x2))
            .ThenByDescending(x => Math.Abs(x.y1 - x.y2))
            .ToList();

        if (filterOverlap)
        {
            List<Volume> blackList = new List<Volume>();
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
        }

        return result.ToArray();
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
        if (Input.GetMouseButtonDown(0) && this.mode != EditingModes.menu)
        {
            Vector3 mp = Input.mousePosition;
            Vector3 pos = this.myCamera.GetComponent<Camera>().ScreenToWorldPoint(mp).OffsetZ(11);

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
            Vector3 pos = this.myCamera.GetComponent<Camera>().ScreenToWorldPoint(mp).OffsetZ(11);
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

    private async void OldSave()
    {
        string tempDir = "E:/Tests/TempDir";
        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
        }

        var files = Directory.GetFiles(tempDir);
        foreach (var item in files)
        {
            File.Delete(item);
        }

        string path = StandaloneFileBrowser.SaveFilePanel("Save File", tempDir, "MyGridGGen", "grid");

        var volumes = await this.GenerateVolumes();

        File.WriteAllText(path, this.toText.ConvertToText(volumes));

        string name = path.Substring(path.LastIndexOf("\\") + 1);

#if UNITY_EDITOR
        string destination = $@"E:\Tests\Grids\{name}";
#else
            string destination = $@"C:\Program Files (x86)\GridMove\Grids\{name}";
#endif
        if (File.Exists(destination))
        {
            File.Delete(destination);
        }

        File.Move(path, destination);
    }

    private async void NewSave()
    {
        if (!this.data.DataIsValid(out AppData data))
        {
            this.ui.DisplayError("Invalid Save data (outputDir and/or gridName)!");
            return;
        }

        var volumes = await this.GenerateVolumes();

        var newFile = $"{data.OutputDir}/{data.GridName}";

        if (File.Exists(Path.Combine(newFile)))
        {
            File.Delete(newFile);
        }

        File.WriteAllText(newFile, this.toText.ConvertToText(volumes));
        this.ui.DisplaySuccess($"File Created at {newFile}", 5000);
    }

    private void NewLoad()
    {
        string defaultPath = string.Empty;

        if (this.data.DataIsValid(out AppData data))
        {
            defaultPath = Path.Combine(data.OutputDir, data.GridName);
        }

        string[] paths = StandaloneFileBrowser.OpenFilePanel(
            "Open File",
            defaultPath,
            "grid",
            false);

        string path = paths[0];

        string fileContents = File.ReadAllText(path);
        var volums = this.toText.ConvertToVolumes(fileContents);

        this.DestroyCurrentSegments();

        this.DrawVolumesSegments(volums);
    }

    private void MyReset()
    {
        DestroyCurrentSegments();
        this.AddBorderLines();
    }

    private void DestroyCurrentSegments()
    {
        for (int i = 0; i < Ysegments.Count; i++)
        {
            this.Ysegments[i].Destroy();
        }
        for (int i = 0; i < Xsegments.Count; i++)
        {
            this.Xsegments[i].Destroy();
        }
        this.Ysegments.Clear();
        this.Xsegments.Clear();
    }

    private void OldLoad()
    {
#if UNITY_EDITOR
        string path = StandaloneFileBrowser.OpenFilePanel(
            "Open File",
            $@"E:\Tests\Grids",
            "grid",
            false)[0];
#else
             string path = StandaloneFileBrowser.OpenFilePanel(
                "Open File",
                $@"C:\Program Files (x86)\GridMove\Grids",
                "grid", 
                false)[0];
#endif

        string fileContents = File.ReadAllText(path);
        var volums = this.toText.ConvertToVolumes(fileContents);

        for (int i = 0; i < Ysegments.Count; i++)
        {
            this.Ysegments[i].Destroy();
        }
        for (int i = 0; i < Xsegments.Count; i++)
        {
            this.Xsegments[i].Destroy();
        }
        this.Ysegments.Clear();
        this.Xsegments.Clear();

        this.DrawVolumesSegments(volums);
    }


    private void CreateLineXPlacing(Vector3 pos, Vector3 pos2)
    {
        var line = new Line();
        line.SetUp(pos, pos2, Color.red, new GameObject().transform, "some", 0.1f, this.CylinderPrefab);
        var seg = line.line.AddComponent<DelimitorSegmentX>();
        seg.SetUp(line, this.Ysegments, this.myCamera, pos.x, new Vector2(pos.x, this.yTop), new Vector2(pos.x, this.yBottom), this);
        this.Xsegments.Add(seg);
    }

    private void CreateLineYPlacing(Vector3 pos, Vector3 pos2)
    {
        var line = new Line();
        line.SetUp(pos, pos2, Color.red, new GameObject().transform, "some", 0.1f, this.CylinderPrefab);
        var seg = line.line.AddComponent<DelimitorSegmentY>();
        seg.SetUp(line, this.Xsegments, this.myCamera, pos.y, new Vector2(this.xLeft, pos.y), new Vector2(this.xRight, pos.y), this);
        this.Ysegments.Add(seg);
    }

    private void CreateLineX(Vector3 pos, Vector3 pos2)
    {
        var line = new Line();
        line.SetUp(pos, pos2, Color.red, new GameObject().transform, "some", 0.1f, this.CylinderPrefab);
        var seg = line.line.AddComponent<DelimitorSegmentX>();
        seg.SetUp(line, this.Ysegments, this.myCamera, pos.x, pos, pos2, this);
        this.Xsegments.Add(seg);
    }

    private void CreateLineY(Vector3 pos, Vector3 pos2)
    {
        var line = new Line();
        line.SetUp(pos, pos2, Color.red, new GameObject().transform, "some", 0.1f, this.CylinderPrefab);
        var seg = line.line.AddComponent<DelimitorSegmentY>();
        seg.SetUp(line, this.Xsegments, this.myCamera, pos.y, pos, pos2, this);
        this.Ysegments.Add(seg);
    }

    #region HELPERS

    private void GetScreenDimentions()
    {
        Vector3 bottomLeft = myCamera.ViewportToWorldPoint(new Vector3(0, 0, myCamera.nearClipPlane));
        Vector3 topRight = myCamera.ViewportToWorldPoint(new Vector3(1, 1, myCamera.nearClipPlane));

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

    private void AddBorderLines()
    {
        this.CreateLineX(new Vector2(xLeft, yBottom), new Vector2(xLeft, yTop));
        this.CreateLineX(new Vector2(xRight, yBottom), new Vector2(xRight, yTop));
        this.CreateLineY(new Vector2(xLeft, yBottom), new Vector2(xRight, yBottom));
        this.CreateLineY(new Vector2(xLeft, yTop), new Vector2(xRight, yTop));
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 25;
        GUI.Label(new Rect(new Vector2(0, 0), new Vector2(200, 200)), this.ID);
    }
}
