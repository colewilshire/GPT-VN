using OpenAI_API.Chat;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : Singleton<SaveController>
{
    private int highestSceneIndex = 0;
    private int maxQuicksaves = 10;
    private int nextQuicksaveIndex = 0;
    public string quicksaveName = "quicksave";
    public string autosaveName = "autosave";

    private string SerializeCharacterAppearance(CharacterAppearance characterAppearance)
    {
        string accessoryName = characterAppearance.Accessory?.name ?? "";
        string hairName = characterAppearance.Hair?.name ?? "";
        string outfitName = characterAppearance.Outfit?.name ?? "";
        string faceName = characterAppearance.Face?.name ?? "";
        string serializedCharacterAppearance = $"{accessoryName}|{hairName}|{outfitName}|{faceName}";

        return serializedCharacterAppearance;
    }

    private Sprite LoadPNGAsSprite(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = File.ReadAllBytes(filePath);
        texture.LoadImage(fileData);

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(texture, rect, pivot);

        return sprite;
    }

    private void SetMostRecentQuicksaveIndex(List<string> sortedSaveFolderPaths)
    {
        if (sortedSaveFolderPaths.Count < 1) return;

        string mostRecentQuicksavePath = sortedSaveFolderPaths.FirstOrDefault(d => Path.GetFileName(d).StartsWith(quicksaveName));
        string mostRecentQuicksaveName = Path.GetFileName(mostRecentQuicksavePath);
        string mostRecentQuicksaveIndexName = mostRecentQuicksaveName.Substring(quicksaveName.Length);

        if (int.TryParse(mostRecentQuicksaveIndexName, out int mostRecentQuicksaveIndex))
        {
            nextQuicksaveIndex = mostRecentQuicksaveIndex + 1;
        }
        else
        {
            nextQuicksaveIndex = 0;
        }
    }

    public void Quicksave()
    {
        if (nextQuicksaveIndex > maxQuicksaves - 1)
        {
            nextQuicksaveIndex = 0;
        }

        Save($"{quicksaveName}{nextQuicksaveIndex}");
        ++nextQuicksaveIndex;
    }

    public void Autosave(int currentSceneIndex)
    {
        if (currentSceneIndex < highestSceneIndex) return;
        highestSceneIndex = currentSceneIndex;
        Save(autosaveName);
    }

    public void Quickload()
    {
        int lastQuicksaveIndex = nextQuicksaveIndex - 1;
        string lastQuicksaveName = $"{quicksaveName}{lastQuicksaveIndex}";
        string savePath = Path.Combine(Application.persistentDataPath, lastQuicksaveName, $"{lastQuicksaveName}.sav");

        if (!(File.Exists(savePath))) return;
        OpenAIController.Instance.LoadConversationFromSave(lastQuicksaveName);
    }

    public void Save(string saveName)
    {
        StateController.Instance.SetState(GameState.Saving);

        string folderPath = Path.Combine(Application.persistentDataPath, saveName);
        string savePath = Path.Combine(folderPath, $"{saveName}.sav");
        string screenshotPath = Path.Combine(folderPath, $"{saveName}.png");
        SaveData saveData = SaveData.CreateInstance<SaveData>();

        saveData.ConversationRoles = new List<string>();
        saveData.ConversationMessages = new List<string>();
        saveData.DialoguePath = DialogueController.Instance.SerializedDialoguePath;
        saveData.CharacterNames = new List<string>();
        saveData.CharacterAppearances = new List<string>();
        saveData.BackgroundIndexes = new List<string>();
        saveData.BackgroundNames = new List<string>();
        saveData.CurrentLineIndex = DialogueController.Instance.CurrentLineIndex;

        foreach(ChatMessage message in OpenAIController.Instance.Chat.Messages)
        {
            saveData.ConversationRoles.Add(message.rawRole);
            saveData.ConversationMessages.Add(message.Content);
        }

        foreach(KeyValuePair<string, CharacterPortraitController> characterEntry in CharacterManager.Instance.Characters)
        {
            CharacterAppearance characterAppearance = characterEntry.Value.Appearance;
            string serializedCharacterAppearance = SerializeCharacterAppearance(characterAppearance);

            saveData.CharacterNames.Add(characterEntry.Key);
            saveData.CharacterAppearances.Add(serializedCharacterAppearance);
        }

        foreach(KeyValuePair<string, BackgroundImage> backgroundEntry in BackgroundController.Instance.GeneratedBackgrounds)
        {
            saveData.BackgroundIndexes.Add(backgroundEntry.Key);
            saveData.BackgroundNames.Add(backgroundEntry.Value.name);
        }

        string serializedSaveData = JsonUtility.ToJson(saveData);

        Directory.CreateDirectory(folderPath);
        File.WriteAllText(savePath, serializedSaveData);
        ScreenCapture.CaptureScreenshot(screenshotPath);
        StateController.Instance.SetState(GameState.Gameplay);
    }

    public SaveData Load(string saveName)
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveName, $"{saveName}.sav");
        if (!File.Exists(savePath)) return null;

        SaveData saveData = SaveData.CreateInstance<SaveData>();
        string serializedSaveData = File.ReadAllText(savePath);

        JsonUtility.FromJsonOverwrite(serializedSaveData, saveData);
        highestSceneIndex = saveData.CurrentLineIndex;

        return saveData;
    }

    public void DeleteSave(string saveName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, saveName);
        Directory.Delete(folderPath, true);
    }

    public Dictionary<string, Sprite> GetSavesSortedByDate()
    {
        string rootSaveFolderPath = Path.Combine(Application.persistentDataPath);
        string[] saveFolderPaths = Directory.GetDirectories(rootSaveFolderPath, "*", SearchOption.TopDirectoryOnly);
        List<string> sortedSaveFolderPaths = saveFolderPaths.OrderByDescending(d => 
            Directory.EnumerateFileSystemEntries(d, "*.*", SearchOption.TopDirectoryOnly)
            .Max(e => (File.GetAttributes(e) & FileAttributes.Directory) == FileAttributes.Directory ? Directory.GetLastWriteTime(e) : File.GetLastWriteTimeUtc(e))
        ).ToList();
        Dictionary<string, Sprite> screenshotDictionary = new Dictionary<string, Sprite>();

        SetMostRecentQuicksaveIndex(sortedSaveFolderPaths);

        foreach(string saveFolderPath in sortedSaveFolderPaths)
        {
            string saveName = Path.GetFileName(saveFolderPath);
            string screenshotPath = Path.Combine(saveFolderPath, $"{saveName}.png");
            Sprite screenshot = LoadPNGAsSprite(screenshotPath);

            screenshotDictionary[saveName] = screenshot;
        }

        return screenshotDictionary;
    }
}
