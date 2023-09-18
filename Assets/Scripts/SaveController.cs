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
        string accessoryName = characterAppearance.accessory?.name ?? "";
        string hairName = characterAppearance.hair?.name ?? "";
        string outfitName = characterAppearance.outfit?.name ?? "";
        string faceName = characterAppearance.face?.name ?? "";
        string serializedCharacterAppearance = $"{accessoryName}|{hairName}|{outfitName}|{faceName}";

        return serializedCharacterAppearance;
    }

    public void Save(string saveName)
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveName + ".sav");
        SaveData saveData = SaveData.CreateInstance<SaveData>();

        saveData.conversationRoles = new List<string>();
        saveData.conversationMessages = new List<string>();
        saveData.dialoguePath = new List<string>();
        saveData.characterNames = new List<string>();
        saveData.characterAppearances = new List<string>();
        saveData.backgroundIndexes = new List<string>();
        saveData.backgroundNames = new List<string>();

        foreach(ChatMessage message in OpenAIController.Instance.Chat.Messages)
        {
            saveData.conversationRoles.Add(message.rawRole);
            saveData.conversationMessages.Add(message.Content);
        }

        foreach(string serializedDialogue in DialogueController.Instance.SerializedDialoguePath)
        {
            saveData.dialoguePath.Add(serializedDialogue);
        }

        foreach(KeyValuePair<string, CharacterPortraitController> characterEntry in CharacterManager.Instance.Characters)
        {
            CharacterAppearance characterAppearance = characterEntry.Value.Appearance;
            string serializedCharacterAppearance = SerializeCharacterAppearance(characterAppearance);

            saveData.characterNames.Add(characterEntry.Key);
            saveData.characterAppearances.Add(serializedCharacterAppearance);
        }

        foreach(KeyValuePair<string, BackgroundImage> backgroundEntry in BackgroundController.Instance.GeneratedBackgrounds)
        {
            saveData.backgroundIndexes.Add(backgroundEntry.Key);
            saveData.backgroundNames.Add(backgroundEntry.Value.name);
        }

        string serializedSaveData = JsonUtility.ToJson(saveData);

        File.WriteAllText(savePath, serializedSaveData);
    }

    public SaveData Load(string saveName)
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveName + ".sav");
        if (!(File.Exists(savePath))) return null;

        SaveData saveData = SaveData.CreateInstance<SaveData>();
        string serializedSaveData = File.ReadAllText(savePath);

        JsonUtility.FromJsonOverwrite(serializedSaveData, saveData);

        return saveData;
    }
}
