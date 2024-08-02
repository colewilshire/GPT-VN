using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using OpenAI_API.Chat;
using UnityEngine;

public class NewSaveController : Singleton<NewSaveController>
{
    private string rootSaveFolderPath;
    private Dictionary<SaveDataType, List<string>> activeSaveData;
    [SerializeField] private string quicksaveName = "quicksave";
    [SerializeField] private string autosaveName = "autosave";

    protected override void Awake()
    {
        base.Awake();

        rootSaveFolderPath = Path.Combine(Application.persistentDataPath, "Saves");
        Directory.CreateDirectory(rootSaveFolderPath);
    }

    public void Save(string saveName)
    {
        string folderPath = Path.Combine(rootSaveFolderPath, saveName);
        Directory.CreateDirectory(folderPath);
    
        string characterDescriptionsPath = Path.Combine(folderPath, $"CharacterDescriptions.json");
        string dialoguePath = Path.Combine(folderPath, $"DialoguePath.json");
        string indexPath = Path.Combine(folderPath, $"CurrentDialogueIndex.json");
        string messagesPath = Path.Combine(folderPath, $"Messages.json");
        string screenshotPath = Path.Combine(folderPath, "Screenshot.png");
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string serializedCharacterDescriptions = JsonSerializer.Serialize<Dictionary<string, CharacterDescription>>(NewCharacterManager.Instance.CharacterDescriptions, jsonSerializerOptions);
        string serializedDialogue = JsonSerializer.Serialize(NewDialogueController.Instance.DialoguePath, jsonSerializerOptions);
        string serializedIndex = JsonSerializer.Serialize(NewDialogueController.Instance.CurrentLineIndex, jsonSerializerOptions);
        string serializedMessages = JsonSerializer.Serialize(NewOpenAIController.Instance.Chat.Messages, jsonSerializerOptions);

        File.WriteAllText(characterDescriptionsPath, serializedCharacterDescriptions, Encoding.Unicode);
        File.WriteAllText(dialoguePath, serializedDialogue, Encoding.Unicode);
        File.WriteAllText(indexPath, serializedIndex, Encoding.Unicode);
        File.WriteAllText(messagesPath, serializedMessages, Encoding.Unicode);
        ScreenCapture.CaptureScreenshot(screenshotPath);
    }

    public NewSaveData LoadSave(string saveName)
    {
        string folderPath = Path.Combine(rootSaveFolderPath, saveName);
        string characterDescriptionsPath = Path.Combine(folderPath, $"CharacterDescriptions.json");
        string dialoguePath = Path.Combine(folderPath, $"DialoguePath.json");
        string indexPath = Path.Combine(folderPath, $"CurrentDialogueIndex.json");
        string messagesPath = Path.Combine(folderPath, $"Messages.json");

        string serializedCharacterDescriptions = File.ReadAllText(characterDescriptionsPath);
        string serializedDialogue = File.ReadAllText(dialoguePath);
        string serializedIndex = File.ReadAllText(indexPath);
        string serializedMessages = File.ReadAllText(messagesPath);

        NewSaveData saveData = new()
        {
            CharacterDescriptions = JsonSerializer.Deserialize<Dictionary<string, CharacterDescription>>(serializedCharacterDescriptions),
            DialoguePath = JsonSerializer.Deserialize<List<NewDialogueLine>>(serializedDialogue),
            CurrentLineIndex = JsonSerializer.Deserialize<int>(serializedIndex),
            Messages = JsonSerializer.Deserialize<IList<ChatMessage>>(serializedMessages)
        };

        return saveData;
    }

    // private Dictionary<string, CharacterDescription> LoadCharacterDescriptions()
    // {
    //     //string savedListPath = Path.Combine(Application.persistentDataPath, $"{saveName}.{SaveFileExtension}");
    //     string path = "C:\\Users\\colew\\AppData\\LocalLow\\DefaultCompany\\GPT-VN\\Saves\\quicksave\\CharacterDescriptions\\CharacterDescriptions0.json";
    //     if (!File.Exists(path))
    //     {
    //         Debug.Log("0");
    //         return null;
    //     }

    //     string serializedCharacterDescriptions = File.ReadAllText(path);
    //     if (serializedCharacterDescriptions == null)
    //     {
    //         Debug.Log("1");
    //         return null;
    //     };

    //     Dictionary<string, CharacterDescription> characterDescriptions = JsonSerializer.Deserialize<Dictionary<string, CharacterDescription>>(serializedCharacterDescriptions);
    //     Debug.Log("2");
    //     return characterDescriptions;
    // }

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

    private Dictionary<SaveDataType, List<string>> CreateNewSaveData()
    {
        Dictionary<SaveDataType, List<string>> newSavaData = new();

        foreach (SaveDataType saveDataType in Enum.GetValues(typeof(SaveDataType)))
        {
            newSavaData[saveDataType] = new();
        }

        return newSavaData;
    }

    public void ClearActiveSaveData()
    {
        activeSaveData = CreateNewSaveData();
    }

    public void CacheData(SaveDataType dataType, string serializedData)
    {
        activeSaveData[dataType].Add(serializedData);
    }

    public void SaveToFile(string saveName)
    {
        string saveFolderPath = Path.Combine(rootSaveFolderPath, saveName);
        string screenshotPath = Path.Combine(saveFolderPath, "Screenshot.png");

        DeleteSaveFile(saveName);
        Directory.CreateDirectory(saveFolderPath);

        if (activeSaveData[SaveDataType.CurrentScene].Count > 0)
        {
            activeSaveData[SaveDataType.CurrentScene][0] = $"{NewDialogueController.Instance.CurrentLineIndex}";
        }
        else
        {
            CacheData(SaveDataType.CurrentScene, $"{NewDialogueController.Instance.CurrentLineIndex}");
        }

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
        //string metadataPath = Path.Combine(saveFolderPath, "CurrentSceneNumber.txt");
        //File.WriteAllText(metadataPath, $"{NewDialogueController.Instance.CurrentLineIndex}");
    }

    // Need to get the CurrentSceneIndex to this
    public Dictionary<SaveDataType, List<string>> LoadSaveFile(string saveName)
    {
        string saveFolderPath = Path.Combine(rootSaveFolderPath, saveName);

        if (!Directory.Exists(saveFolderPath))
        {
            return null;
        }

        Dictionary<SaveDataType, List<string>> loadedSaveData = CreateNewSaveData();

        foreach (SaveDataType saveDataType in Enum.GetValues(typeof(SaveDataType)))
        {
            string dataFolderPath = Path.Combine(saveFolderPath, saveDataType.ToString());
            List<string> filePaths = Directory.GetFiles(dataFolderPath).ToList();

            foreach (string filePath in filePaths)
            {
                string fileContents = File.ReadAllText(filePath);
                loadedSaveData[saveDataType].Add(fileContents);
            }
        }

        return loadedSaveData;
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

    public void Quicksave()
    {
        Save(quicksaveName);
    }

    public void Autosave()
    {
        Save(autosaveName);
    }

    public void Quickload()
    {
        NewOpenAIController.Instance.LoadConversationFromSave(quicksaveName);
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
