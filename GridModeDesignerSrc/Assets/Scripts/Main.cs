using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private InputField s;
    private InputField l;
    void Start()
    {
        this.s = GameObject.Find("ShortInp").GetComponent<InputField>();
        this.l = GameObject.Find("LongInp").GetComponent<InputField>();

        var btn = GameObject.Find("SetBtn");
        btn.GetComponent<Button>().onClick.AddListener(this.OnClickSet);
    }

    private void OnClickSet()
    {
        try
        {
            var ll = int.Parse(this.l.text);
            var ss = int.Parse(this.s.text);

            Debug.Log($"{ll} {ss}"); 
        }
        catch (Exception e) 
        {
            Debug.Log($"error");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private DrawLine()
    {

    }
}
