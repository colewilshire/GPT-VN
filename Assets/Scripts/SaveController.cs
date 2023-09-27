using OpenAI_API.Chat;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : Singleton<SaveController>
{
    public string quicksaveName = "quicksave";
    public string autosaveName = "autosave";

    public void Quicksave()
    {
        Save(quicksaveName);
    }

    // public void Autosave()
    // {
    //     Save(autosaveName);
    // }

    // public void Quickload()
    // {
    //     string savePath = Path.Combine(Application.persistentDataPath, quicksaveName + ".sav");
    //     if (!(File.Exists(savePath))) return;
    // }

    private string SerializeCharacterAppearance(CharacterAppearance characterAppearance)
    {
        string accessoryName = characterAppearance.Accessory?.name ?? "";
        string hairName = characterAppearance.Hair?.name ?? "";
        string outfitName = characterAppearance.Outfit?.name ?? "";
        string faceName = characterAppearance.Face?.name ?? "";
        string serializedCharacterAppearance = $"{accessoryName}|{hairName}|{outfitName}|{faceName}";

        return serializedCharacterAppearance;
    }

    public void Save(string saveName)
    {
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

        File.WriteAllText(savePath, serializedSaveData);
        ScreenCapture.CaptureScreenshot(screenshotPath);
    }

    public SaveData Load(string saveName)
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveName, $"{saveName}.sav");
        if (!(File.Exists(savePath))) return null;

        SaveData saveData = SaveData.CreateInstance<SaveData>();
        string serializedSaveData = File.ReadAllText(savePath);

        JsonUtility.FromJsonOverwrite(serializedSaveData, saveData);

        return saveData;
    }
}
