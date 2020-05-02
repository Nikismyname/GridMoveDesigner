using SFB;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiManager : MonoBehaviour
{
    Button menuButton;
    Button hideButton;
    Button saveButton;

    GameObject mainPanel;

    Button exploreTemp;
    Button exploreOutput;

    InputField tempIF;
    InputField outputIF;

    Text displayText;

    private DataManager data;

    private void Start()
    {
        this.data = new DataManager();

        this.hideButton = GameObject.Find("HideButton").GetComponent<Button>();
        this.hideButton.onClick.AddListener(this.OnClickHide);

        this.mainPanel = GameObject.Find("MainPanel");

        this.exploreTemp = GameObject.Find("TempDir").transform.Find("Button").GetComponent<Button>();
        this.exploreOutput = GameObject.Find("OutputDir").transform.Find("Button").GetComponent<Button>();
        this.exploreTemp.onClick.AddListener(() => this.OnClickExplore(true));
        this.exploreOutput.onClick.AddListener(() => this.OnClickExplore(false));

        this.tempIF = GameObject.Find("TempDir").transform.Find("InputField").GetComponent<InputField>();
        this.outputIF = GameObject.Find("OutputDir").transform.Find("InputField").GetComponent<InputField>();

        this.displayText = GameObject.Find("DisplayText").GetComponent<Text>();

        this.menuButton = GameObject.Find("MenuButton").GetComponent<Button>();
        this.menuButton.onClick.AddListener(this.OnClickMenuButton);
        this.menuButton.gameObject.SetActive(false);

        this.saveButton = GameObject.Find("SaveButton").GetComponent<Button>();
        this.saveButton.onClick.AddListener(this.OnClickSaveButton);

        var data = this.data.DeserializeBoth();
        this.outputIF.text = data.OutputDir;
        this.tempIF.text = data.TempDir;
    }

    private void OnClickSaveButton()
    {
        string tempPath = this.tempIF.text;
        string outputPath = this.outputIF.text;

        if (!Directory.Exists(outputPath))
        {
            this.displayText.text += Environment.NewLine + "Output path does not exist!";
            return; 
        }

        if (!Directory.Exists(tempPath))
        {
            this.displayText.text += Environment.NewLine + "Temp path  does not exist!";
            return;
        }

        this.data.SerializeBoth(tempPath, outputPath);
    }

    private void OnClickHide()
    {
        this.mainPanel.SetActive(false);
        this.menuButton.gameObject.SetActive(true);
    }

    private void OnClickMenuButton()
    {
        this.mainPanel.SetActive(true);
        this.menuButton.gameObject.SetActive(false);
    }

    private void OnClickExplore(bool isTemp)
    {
        string[] selectedDirs = StandaloneFileBrowser.OpenFolderPanel("Select Directory", "", false);
        if(selectedDirs.Length == 1)
        {
            string dir = selectedDirs[0];

            if (!Directory.Exists(dir))
            {
                this.displayText.text += Environment.NewLine + "This path does not exist!";
                return;
            }

            if (isTemp)
            {
                this.tempIF.text= dir;
            }
            else
            {
                this.outputIF.text = dir;
            }
        }
        else
        {
            Debug.Log(selectedDirs.Length);
            return; 
        }
    }
}

