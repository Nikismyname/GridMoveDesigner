using TMPro;
using UnityEngine;

public class Some : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    // Use this for initialization
    void Start()
    {
        textMesh = GameObject.Find("Some").GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        //if (textMesh.text.Length > 300)
        //{
        //    textMesh.text = message + "\n";
        //}
        //else
        //{
        //    textMesh.text += message + "\n";
        //}

        if (textMesh != null)
        {
            textMesh.text += message + "\n";
        }
        else
        {
            Debug.Log("NOT FOUND!");
        }
    }
}
