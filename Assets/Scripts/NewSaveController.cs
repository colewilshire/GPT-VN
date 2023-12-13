using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NewSaveController : Singleton<NewSaveController>
{
    private string rootSaveFolderPath;
    private Dictionary<SaveDataType, List<string>> activeSaveData;

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
        activeSaveData = new();

        foreach (SaveDataType saveDataType in Enum.GetValues(typeof(SaveDataType)))
        {
            activeSaveData[saveDataType] = new();
        }
    }

    public void SaveData(SaveDataType dataType, string serializedData)
    {
        activeSaveData[dataType].Add(serializedData);
    }

    public void SaveToFile(string saveName)
    {
        string saveFolderPath = Path.Combine(rootSaveFolderPath, saveName);
        string screenshotPath = Path.Combine(saveFolderPath, "Screenshot.png");

        DeleteSaveFile(saveName);
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

    public Dictionary<SaveDataType, List<string>> LoadSaveFile(string saveName)
    {
        string saveFolderPath = Path.Combine(rootSaveFolderPath, saveName);

        if (!Directory.Exists(saveFolderPath))
        {
            return null;
        }

        ClearActiveSaveData();

        foreach (SaveDataType saveDataType in Enum.GetValues(typeof(SaveDataType)))
        {
            string dataFolderPath = Path.Combine(saveFolderPath, saveDataType.ToString());
            List<string> filePaths = Directory.GetFiles(dataFolderPath).ToList();

            foreach (string filePath in filePaths)
            {
                string fileContents = File.ReadAllText(filePath);
                activeSaveData[saveDataType].Add(fileContents);
            }
        }

        return activeSaveData;
    }

    public void DeleteSaveFile(string saveName)
    {
        string saveFolderPath = Path.Combine(rootSaveFolderPath, saveName);
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
