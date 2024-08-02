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
    [SerializeField] private string quicksaveName = "quicksave";
    [SerializeField] private string autosaveName = "autosave";
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
    
        string characterDescriptionsPath = Path.Combine(folderPath, $"CharacterDescriptions.json");
        string dialoguePath = Path.Combine(folderPath, $"DialoguePath.json");
        string indexPath = Path.Combine(folderPath, $"CurrentDialogueIndex.json");
        string messagesPath = Path.Combine(folderPath, $"Messages.json");
        string screenshotPath = Path.Combine(folderPath, "Screenshot.png");
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new ChatMessageRoleConverter() }
        };

        string serializedCharacterDescriptions = JsonSerializer.Serialize(NewCharacterManager.Instance.CharacterDescriptions, jsonSerializerOptions);
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

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new ChatMessageRoleConverter() }
        };
        NewSaveData saveData = new()
        {
            CharacterDescriptions = JsonSerializer.Deserialize<Dictionary<string, CharacterDescription>>(serializedCharacterDescriptions, jsonSerializerOptions),
            DialoguePath = JsonSerializer.Deserialize<List<NewDialogueLine>>(serializedDialogue, jsonSerializerOptions),
            CurrentLineIndex = JsonSerializer.Deserialize<int>(serializedIndex, jsonSerializerOptions),
            Messages = JsonSerializer.Deserialize<IList<ChatMessage>>(serializedMessages, jsonSerializerOptions)
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
}
