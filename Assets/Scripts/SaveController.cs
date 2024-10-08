using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using OpenAI.Chat;
using UnityEngine;

public class SaveController : Singleton<SaveController>
{
    [Header("Save Names")]
    [SerializeField] private string quicksaveName = "quicksave";
    [SerializeField] private string autosaveName = "autosave";
    [SerializeField] private string genreSaveName = "Genre.json";
    [SerializeField] private string settingSaveName = "Setting.json";
    [SerializeField] private string protagonistNameSaveName = "ProtagonistName.json";
    [SerializeField] private string characterDescriptionsSaveName = "CharacterDescriptions.json";
    [SerializeField] private string dialogueSaveName = "DialoguePath.json";
    [SerializeField] private string indexSaveName = "CurrentDialogueIndex.json";
    [SerializeField] private string messagesSaveName = "Messages.json";
    [SerializeField] private string screenshotSaveName = "Screenshot.png";

    private string rootSaveFolderPath;

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

    public void Save(string saveName)
    {
        string folderPath = Path.Combine(rootSaveFolderPath, saveName);
        Directory.CreateDirectory(folderPath);
    
        string genrePath = Path.Combine(folderPath, genreSaveName);
        string settingPath = Path.Combine(folderPath, settingSaveName);
        string protagonistNamePath = Path.Combine(folderPath, protagonistNameSaveName);
        string characterDescriptionsPath = Path.Combine(folderPath, characterDescriptionsSaveName);
        string dialoguePath = Path.Combine(folderPath, dialogueSaveName);
        string indexPath = Path.Combine(folderPath, indexSaveName);
        string messagesPath = Path.Combine(folderPath, messagesSaveName);
        string screenshotPath = Path.Combine(folderPath, screenshotSaveName);
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        string serializedGenre = JsonSerializer.Serialize(OpenAIController.Instance.Genre, jsonSerializerOptions);
        string serializedSetting = JsonSerializer.Serialize(OpenAIController.Instance.Setting, jsonSerializerOptions);
        string serializedProtagonistName = JsonSerializer.Serialize(OpenAIController.Instance.ProtagonistName, jsonSerializerOptions);
        string serializedCharacterDescriptions = JsonSerializer.Serialize(CharacterManager.Instance.CharacterDescriptions, jsonSerializerOptions);
        string serializedDialogue = JsonSerializer.Serialize(DialogueController.Instance.DialoguePath, jsonSerializerOptions);
        string serializedIndex = JsonSerializer.Serialize(DialogueController.Instance.CurrentLineIndex, jsonSerializerOptions);
        string serializedMessages = JsonSerializer.Serialize(OpenAIController.Instance.MessageDictionary, jsonSerializerOptions);

        File.WriteAllText(genrePath, serializedGenre, Encoding.Unicode);
        File.WriteAllText(settingPath, serializedSetting, Encoding.Unicode);
        File.WriteAllText(protagonistNamePath, serializedProtagonistName, Encoding.Unicode);
        File.WriteAllText(characterDescriptionsPath, serializedCharacterDescriptions, Encoding.Unicode);
        File.WriteAllText(dialoguePath, serializedDialogue, Encoding.Unicode);
        File.WriteAllText(indexPath, serializedIndex, Encoding.Unicode);
        File.WriteAllText(messagesPath, serializedMessages, Encoding.Unicode);
        ScreenCapture.CaptureScreenshot(screenshotPath);
    }

    public SaveData LoadSave(string saveName)
    {
        string folderPath = Path.Combine(rootSaveFolderPath, saveName);
        string genrePath = Path.Combine(folderPath, genreSaveName);
        string settingPath = Path.Combine(folderPath, settingSaveName);
        string protagonistNamePath = Path.Combine(folderPath, protagonistNameSaveName);
        string characterDescriptionsPath = Path.Combine(folderPath, characterDescriptionsSaveName);
        string dialoguePath = Path.Combine(folderPath, dialogueSaveName);
        string indexPath = Path.Combine(folderPath, indexSaveName);
        string messagesPath = Path.Combine(folderPath, messagesSaveName);

        string serializedGenre = File.ReadAllText(genrePath);
        string serializedSetting = File.ReadAllText(settingPath);
        string serializedProtagonistName = File.ReadAllText(protagonistNamePath);
        string serializedCharacterDescriptions = File.ReadAllText(characterDescriptionsPath);
        string serializedDialogue = File.ReadAllText(dialoguePath);
        string serializedIndex = File.ReadAllText(indexPath);
        string serializedMessages = File.ReadAllText(messagesPath);

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        SaveData saveData = new()
        {
            Genre = JsonSerializer.Deserialize<string>(serializedGenre, jsonSerializerOptions),
            Setting = JsonSerializer.Deserialize<string>(serializedSetting, jsonSerializerOptions),
            ProtagonistName = JsonSerializer.Deserialize<string>(serializedProtagonistName, jsonSerializerOptions),
            CharacterDescriptions = JsonSerializer.Deserialize<Dictionary<string, CharacterDescription>>(serializedCharacterDescriptions, jsonSerializerOptions),
            DialoguePath = JsonSerializer.Deserialize<List<DialogueLine>>(serializedDialogue, jsonSerializerOptions),
            CurrentLineIndex = JsonSerializer.Deserialize<int>(serializedIndex, jsonSerializerOptions),
            Messages = JsonSerializer.Deserialize<List<KeyValuePair<ChatMessageRole, string>>>(serializedMessages, jsonSerializerOptions)
        };

        return saveData;
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
            string screenshotPath = Path.Combine(saveFolderPath, screenshotSaveName);
            Sprite screenshot = LoadPNGAsSprite(screenshotPath);

            screenshotDictionary[saveName] = screenshot;
        }

        return screenshotDictionary;
    }

    public void DeleteSave(string saveName)
    {
        string folderPath = Path.Combine(rootSaveFolderPath, saveName);
        Directory.Delete(folderPath, true);
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
        OpenAIController.Instance.LoadConversationFromSave(quicksaveName);
    }
}
