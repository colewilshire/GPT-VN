using OpenAI_API.Chat;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : Singleton<SaveController>
{
    public string quicksaveName = "quicksave";
    public string autosaveName = "autosave";
    public SaveData saveData { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        saveData = SaveData.CreateInstance<SaveData>();
    }

    // public void Save(string saveName)
    // {
    //     saveData.roles = new List<string>();
    //     saveData.messages = new List<string>();

    //     foreach(ChatMessage message in OpenAIController.Instance.Chat.Messages)
    //     {
    //         saveData.roles.Add(message.rawRole);
    //         saveData.messages.Add(message.Content);
    //     }

    //     string savePath = Path.Combine(Application.persistentDataPath, saveName + ".sav");
    //     string json = JsonUtility.ToJson(saveData);

    //     File.WriteAllText(savePath, json);
    // }

    // public SaveData Load(string saveName)
    // {
    //     string savePath = Path.Combine(Application.persistentDataPath, saveName + ".sav");

    //     if (File.Exists(savePath))
    //     {
    //         string json = File.ReadAllText(savePath);
    //         JsonUtility.FromJsonOverwrite(json, saveData);

    //         return saveData;
    //     }

    //     return null;
    // }

    // public void Quicksave()
    // {
    //     Save(quicksaveName);
    // }

    // public void Autosave()
    // {
    //     Save(autosaveName);
    // }

    // public void Quickload()
    // {
    //     string savePath = Path.Combine(Application.persistentDataPath, quicksaveName + ".sav");
    //     if (!(File.Exists(savePath))) return;
    // }
}
