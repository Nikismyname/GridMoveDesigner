using System.IO;
using UnityEngine;

public class DataManager
{

#if UNITY_EDITOR
    public const string fileName = @"E:\Tests\NewGrids\SomeData.txt";
#else
    public const string fileName = "SomeData.txt";
#endif

    public DataManager()
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

    public void SerializeTempDir(string tempDir)
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

        data.TempDir = tempDir;

        File.WriteAllText(fileName, JsonUtility.ToJson(data));
    }

    public void SerializeBoth(string temp, string output)
    {
        AppData data = new AppData
        {
            OutputDir = output,
            TempDir = temp
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
}
