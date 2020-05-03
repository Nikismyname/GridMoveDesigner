#region INIT

using SFB;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiManager : MonoBehaviour
{
    private Button menuButton;
    private Button hideButton;
    private Button saveButton;
    private GameObject mainPanel;
    private Button exploreOutput;
    private InputField girdNameIF;
    private InputField outputIF;
    private Text displayText;
    private Image messageImage;
    private Text messageText;

    private DataManager data;
    private Main main;

    private void Start()
    {
        this.data = new DataManager();
        this.main = gameObject.GetComponent<Main>();

        this.hideButton = GameObject.Find("HideButton").GetComponent<Button>();
        this.hideButton.onClick.AddListener(this.OnHideUI);

        this.mainPanel = GameObject.Find("MainPanel");

        this.exploreOutput = GameObject.Find("OutputDir").transform.Find("Button").GetComponent<Button>();
        this.exploreOutput.onClick.AddListener(() => this.OnClickExplore(false));

        this.girdNameIF = GameObject.Find("GridName").transform.Find("InputField").GetComponent<InputField>();
        this.outputIF = GameObject.Find("OutputDir").transform.Find("InputField").GetComponent<InputField>();

        this.displayText = GameObject.Find("DisplayText").GetComponent<Text>();

        this.menuButton = GameObject.Find("MenuButton").GetComponent<Button>();
        this.menuButton.onClick.AddListener(this.OnEnableMenu);
        this.menuButton.gameObject.SetActive(false);

        this.saveButton = GameObject.Find("SaveButton").GetComponent<Button>();
        this.saveButton.onClick.AddListener(this.OnClickSaveButton);

        bool dataIsValid = this.data.DataIsValid(out AppData data);
        if (dataIsValid)
        {
            this.outputIF.text = data.OutputDir;
            this.girdNameIF.text = data.GridName;
        }
        else
        {
            this.outputIF.text = "";
            this.girdNameIF.text = "";
        }

        GameObject messagePanel = GameObject.Find("MessagePanel");
        this.messageImage = messagePanel.GetComponent<Image>();
        this.messageText = messagePanel.GetComponentInChildren<Text>();
        messagePanel.SetActive(false);

        if (dataIsValid)
        {
            this.OnHideUI();
        }

        Debug.Log("UI<MAN>");
    }

    #endregion

    #region SAVE BTN

    private void OnClickSaveButton()
    {
        Debug.Log("SAVING1"); 
        string gridName = this.girdNameIF.text;
        string outputPath = this.outputIF.text;

        if (!Directory.Exists(outputPath))
        {
            this.displayText.text += Environment.NewLine + "Output path does not exist!";
            this.DisplayError("Output path does not exist!");
            return;
        }
        Debug.Log("SAVING2");

        if (string.IsNullOrEmpty(gridName))
        {
            this.displayText.text += Environment.NewLine + "Grid Name Not Valid!";
            this.DisplayError("Grid Name Not Valid!");
            return;
        }
        Debug.Log("SAVING3");

        if (!gridName.EndsWith(".grid"))
        {
            gridName += ".grid";
        }
        Debug.Log("SAVING4");

        this.data.SerializeBoth(gridName, outputPath);
        Debug.Log("SAVING5");
        this.OnHideUI();
        this.DisplaySuccess("Info Saved!");
        Debug.Log("SAVING6");
    }

    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) &&  this.displayingMessage)
        {
            this.StopDisplayingMessage();
        }
    }

    public void OnHideUI()
    {
        if (this.data.DataIsValid() == false)
        {
            Debug.Log("Data is not valid can not close the dialog!");
            return;
        }

        this.mainPanel.SetActive(false);
        this.main.mode = EditingModes.placingLines;
    }

    public void OnEnableMenu()
    {
        this.mainPanel.SetActive(true);
        this.menuButton.gameObject.SetActive(false);
    }

    private void OnClickExplore(bool isTemp)
    {
        string[] selectedDirs = StandaloneFileBrowser.OpenFolderPanel("Select Directory", "", false);
        if (selectedDirs.Length == 1)
        {
            string dir = selectedDirs[0];

            if (!Directory.Exists(dir))
            {
                this.displayText.text += Environment.NewLine + "This path does not exist!";
                return;
            }

            if (isTemp)
            {
                //this.tempIF.text= dir;
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

    #region MESSAGES

    bool displayingMessage = false;
    private void DisplayMessage(string message, int ms)
    {
        Debug.Log($"Trying to display message");
        if (this.displayingMessage == true)
        {
            Debug.Log("Already displaying message!");
            return;
        }
        this.messageText.text = message;
        this.messageImage.gameObject.SetActive(true);
        this.displayingMessage = true;
        this.main.mode = EditingModes.menu;
        //await Task.Delay(ms);
        //this.messageImage.gameObject.SetActive(false);
        //this.displayingMessage = false;
        //this.main.mode = EditingModes.placingLines;
    }

    private void StopDisplayingMessage()
    {
        this.messageImage.gameObject.SetActive(false);
        this.displayingMessage = false;
        this.main.mode = EditingModes.placingLines;
    }
    public void DisplayError(string message, int ms = 1000)
    {
        this.messageImage.color = Color.red;
        this.DisplayMessage(message, ms);
    }
    public void DisplaySuccess(string message, int ms = 1000)
    {
        this.messageImage.color = Color.green;
        this.DisplayMessage(message, ms);
    }

    #endregion

    private void OnGUI()
    {
        GUI.Label(new Rect(new Vector2(0, 0), new Vector2(500, 500)), "[M] for menu!");
    }
}

