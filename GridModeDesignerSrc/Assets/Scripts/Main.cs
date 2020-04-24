using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    List<DelimitorSegment> segments = new List<DelimitorSegment>();
    private Camera camera;
    void Start()
    {
        //this.s = GameObject.Find("ShortInp").GetComponent<InputField>();
        //this.l = GameObject.Find("LongInp").GetComponent<InputField>();
        this.camera = Camera.main;

        var btn = GameObject.Find("SetBtn");
        //btn.GetComponent<Button>().onClick.AddListener(this.OnClickSet);
    }

    private void OnClickSet()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            //this.segments[this.segments.Count - 1].remove(); 
        }

        bool horizontal = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            horizontal = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mp = Input.mousePosition;
            Vector3 pos = this.camera.GetComponent<Camera>().ScreenToWorldPoint(mp).OffsetZ(11);
            if(this.segments.Any(x=>x.isColliding(pos)))
            {
                Debug.Log("OVER");
                return;
            }
            else
            {
                Debug.Log("NOT OVER!");
            }

            Vector3 bottomRight = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            Vector3 topLeft = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

            DrawX(bottomRight, Color.green);
            DrawX(topLeft, Color.red);

            var line = new Line();

            if (horizontal == false)
            {
                Vector3 pos2 = new Vector3(pos.x, bottomRight.y, pos.z);
                pos = new Vector3(pos.x, topLeft.y, pos.z);
                line.SetUp(pos, pos2, Color.black, new GameObject().transform, "some", 0.1f);
                var seg = line.line.AddComponent<DelimitorSegment>();
                seg.SetUp(line, this.camera, false, pos.x);
                this.segments.Add(seg);

                Debug.Log("Horizontal Kreated!");
            }
            else
            {
                Vector3 pos2 = new Vector3(bottomRight.x, pos.y, pos.z);
                pos = new Vector3(topLeft.x, pos.y, pos.z);
                line.SetUp(pos, pos2, Color.black, new GameObject().transform, "some", 0.1f);
                var seg = line.line.AddComponent<DelimitorSegment>();
                seg.SetUp(line, this.camera, true, pos.y);
                this.segments.Add(seg);

                Debug.Log("Horizontal Kreated!");
            }
        }
    }

    private void DrawX(Vector3 pos, Color color)
    {
        Debug.DrawLine(pos.OffsetY(0.5f), pos.OffsetY(-0.5f), color, 0.3f);
        Debug.DrawLine(pos.OffsetX(0.5f), pos.OffsetX(-0.5f), color, 0.3f);
    }
}
