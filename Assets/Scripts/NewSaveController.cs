using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NewSaveController : Singleton<NewSaveController>
{
    private string rootSaveFolderPath;
    private Dictionary<string, List<string>> serializedSaveData = new()
    {
        ["CharacterDescriptions"] = new(),
        ["DialogueScenes"] = new()
    };

    protected override void Awake()
    {
        base.Awake();

        rootSaveFolderPath = Path.Combine(Application.persistentDataPath, "Saves");
    }

    public void LoadSave()
    {
        foreach(KeyValuePair<string, List<string>> keyValuePair in serializedSaveData)
        {
            serializedSaveData[keyValuePair.Key] = new();
        }
    }

    public void SaveData(string dataType, string serializedData)
    {
        serializedSaveData[dataType].Add(serializedData);
    }

    public void SaveToFile(string fileName)
    {
        string saveFolderPath = Path.Combine(rootSaveFolderPath, fileName);
        string screenshotPath = Path.Combine(saveFolderPath, $"{fileName}.png");
        Directory.CreateDirectory(saveFolderPath);

        foreach(KeyValuePair<string, List<string>> keyValuePair in serializedSaveData)
        {
            string dataFolderPath = Path.Combine(saveFolderPath, keyValuePair.Key);
            List<string> dataList = keyValuePair.Value;
            Directory.CreateDirectory(dataFolderPath);

            for (int i = 0; i < dataList.Count; ++i)
            {
                string dataPath = Path.Combine(dataFolderPath, $"{keyValuePair.Key}{i}.json");
                File.WriteAllText(dataPath, dataList[i]);
            }
        }

        ScreenCapture.CaptureScreenshot(screenshotPath);
    }

    public void DeleteSaveFile(string saveName)
    {
        string folderPath = Path.Combine(rootSaveFolderPath, saveName);
        Directory.Delete(folderPath, true);
    }
}
