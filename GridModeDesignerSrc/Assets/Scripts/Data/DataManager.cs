using System.IO;
using UnityEngine;

public class DataManager
{

#if UNITY_EDITOR
    public const string fileName = @"E:\Tests\NewGrids\SomeData.txt";
#else
    public const string fileName = "SomeData.txt";
#endif

    public DataManager() { }

    public void InitializeFile()
    {
        if(File.Exists(fileName) == false)
        {
            File.Create(fileName); 
        }
    }

    public void SerializeOutputDir(string outputDir)
    {
        var text = File.ReadAllText(fileName);
        AppData data = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            data = new AppData();
        }
        else
        {
            data = JsonUtility.FromJson<AppData>(text);
        }

        data.OutputDir = outputDir;

        File.WriteAllText(fileName, JsonUtility.ToJson(data));
    }

    public void SerializeGridName(string tempDir)
    {
        var text = File.ReadAllText(fileName);
        AppData data = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            data = new AppData();
        }
        else
        {
            data = JsonUtility.FromJson<AppData>(text);
        }

        data.GridName = tempDir;

        File.WriteAllText(fileName, JsonUtility.ToJson(data));
    }

    public void SerializeBoth(string grid, string output)
    {
        AppData data = new AppData
        {
            OutputDir = output,
            GridName = grid
        };

        File.WriteAllText(fileName, JsonUtility.ToJson(data));
    }
    
    public AppData DeserializeBoth()
    {
        var text = File.ReadAllText(fileName);
        if (string.IsNullOrWhiteSpace(text))
        {
            return new AppData(); 
        }

        var data = JsonUtility.FromJson<AppData>(text);
        return data;
    }

    public bool DataIsValid()
    {
        return this.DataIsValid(out AppData _);
    }

    public bool DataIsValid(out AppData outData)
    {
        outData = null;
        var data = this.DeserializeBoth(); 

        if(string.IsNullOrEmpty(data.GridName) || data.GridName.EndsWith(".grid") == false)
        {
            Debug.Log("grid name no valid!");
            return false;
        }

        if(string.IsNullOrEmpty(data.OutputDir) || Directory.Exists(data.OutputDir) == false)
        {
            Debug.Log("output dir not valid");
            return false;
        }

        outData = data;
        return true;
    }
}
