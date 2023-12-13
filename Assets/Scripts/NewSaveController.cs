using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NewSaveController : Singleton<NewSaveController>
{
    private string rootSaveFolderPath;
    private Dictionary<SaveDataType, List<string>> activeSaveData = new()
    {
        [SaveDataType.CharacterDescriptions] = new(),
        [SaveDataType.DialogueScenes] = new()
    };

    protected override void Awake()
    {
        base.Awake();

        rootSaveFolderPath = Path.Combine(Application.persistentDataPath, "Saves");
        Directory.CreateDirectory(rootSaveFolderPath);
    }

    private Sprite LoadPNGAsSprite(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        Texture2D texture = new(2, 2);
        byte[] fileData = File.ReadAllBytes(filePath);
        texture.LoadImage(fileData);

        Rect rect = new(0, 0, texture.width, texture.height);
        Vector2 pivot = new(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(texture, rect, pivot);

        return sprite;
    }

    public void ClearActiveSaveData()
    {
        activeSaveData = new()
        {
            [SaveDataType.CharacterDescriptions] = new(),
            [SaveDataType.DialogueScenes] = new()
        };
    }

    public void SaveData(SaveDataType dataType, string serializedData)
    {
        activeSaveData[dataType].Add(serializedData);
    }

    public void SaveToFile(string fileName)
    {
        string saveFolderPath = Path.Combine(rootSaveFolderPath, fileName);
        string screenshotPath = Path.Combine(saveFolderPath, "Screenshot.png");

        DeleteSaveFile(fileName);
        Directory.CreateDirectory(saveFolderPath);

        foreach(KeyValuePair<SaveDataType, List<string>> keyValuePair in activeSaveData)
        {
            string dataFolderPath = Path.Combine(saveFolderPath, keyValuePair.Key.ToString());
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

    public void DeleteSaveFile(string fileName)
    {
        string saveFolderPath = Path.Combine(rootSaveFolderPath, fileName);
        if (Directory.Exists(saveFolderPath))
        {
            Directory.Delete(saveFolderPath, true);
        }
    }

    public Dictionary<string, Sprite> GetSavesSortedByDate()
    {
        Directory.CreateDirectory(rootSaveFolderPath);
        List<string> saveFolderPaths = Directory.GetDirectories(rootSaveFolderPath, "*", SearchOption.TopDirectoryOnly).ToList();
        IOrderedEnumerable<string> orderedSaveFolderPaths = saveFolderPaths.OrderByDescending(path => Directory.GetLastAccessTime(path));
        Dictionary<string, Sprite> screenshotDictionary = new();

        foreach (string saveFolderPath in orderedSaveFolderPaths)
        {
            string saveName = Path.GetFileName(saveFolderPath);
            string screenshotPath = Path.Combine(saveFolderPath, "Screenshot.png");
            Sprite screenshot = LoadPNGAsSprite(screenshotPath);

            screenshotDictionary[saveName] = screenshot;
        }

        return screenshotDictionary;
    }

    // public Dictionary<string, Sprite> GetSavesSortedByDate2()
    // {
    //     Directory.CreateDirectory(rootSaveFolderPath);
    //     List<string> saveFolderPaths = Directory.GetDirectories(rootSaveFolderPath, "*", SearchOption.TopDirectoryOnly).ToList();
    //     List<KeyValuePair<string, DateTime>> saveFolderDates = new();

    //     foreach (string saveFolderPath in saveFolderPaths)
    //     {
    //         string screenshotPath = Path.Combine(saveFolderPath, "Screenshot.png");

    //         if (File.Exists(screenshotPath))
    //         {
    //             DateTime lastWriteTime = File.GetLastWriteTime(screenshotPath);
    //             saveFolderDates.Add(new KeyValuePair<string, DateTime>(saveFolderPath, lastWriteTime));
    //         }
    //     }

    //     saveFolderDates.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
    //     Dictionary<string, Sprite> screenshotDictionary = new();

    //     foreach (KeyValuePair<string, DateTime> keyValuePair in saveFolderDates)
    //     {
    //         string saveName = Path.GetFileName(keyValuePair.Key);
    //         string screenshotPath = Path.Combine(keyValuePair.Key, "Screenshot.png");
    //         Sprite screenshot = LoadPNGAsSprite(screenshotPath);

    //         screenshotDictionary[saveName] = screenshot;
    //     }

    //     return screenshotDictionary;
    // }
}
