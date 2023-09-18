using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    private List<DialogueLine> dialoguePath = new List<DialogueLine>();
    private int currentLineIndex = 0;
    public List<string> SerializedDialoguePath {get; private set;} = new List<string>();

    private void ReadDialogueLine(int index)
    {
        DialogueLine currentLine = dialoguePath[index];

        BackgroundController.Instance.SetBackground(currentLine.backgroundImage);
        CharacterManager.Instance.ShowPortait(currentLine.characterName, currentLine.mood);
        NameDisplayController.Instance.SetDisplayName(currentLine.characterName);
        TextController.Instance.SetText(currentLine.dialogueText);
        AudioController.Instance.PlaySound(currentLine.voiceLine);
    }

    private DialogueLine DeserializeLine(string serializedLine)
    {
        List<string> splitLine = new List<string>(serializedLine.Split('|', StringSplitOptions.RemoveEmptyEntries))
            .Select(str => str.Trim())
            .Select(str => str.Trim('"'))
            .ToList();
        DialogueLine dialogueLine = DialogueLine.CreateInstance<DialogueLine>();
        string characterName = splitLine[0];
        string dialogueText = splitLine[1];
        string characterMood = splitLine[2];
        string backgroundName = splitLine[3];

        dialogueLine.characterName = characterName;
        dialogueLine.dialogueText = dialogueText;
        dialogueLine.mood = characterMood;
        dialogueLine.backgroundImage = BackgroundController.Instance.GetBackgroundImageFromName(backgroundName);

        return dialogueLine;
    }

    public void StepForward()
    {
        if (!(dialoguePath.Count > currentLineIndex + 1)) return;
        currentLineIndex += 1;
        ReadDialogueLine(currentLineIndex);
    }

    public void StepBackward()
    {
        if (!(currentLineIndex - 1 >= 0)) return;
        currentLineIndex -= 1;
        ReadDialogueLine(currentLineIndex);
    }

    public void RepeatLine()
    {
        if (!(dialoguePath.Count > 0)) return;
        ReadDialogueLine(currentLineIndex);
    }

    public void StartDialogue(string serializedDialouge)
    {
        dialoguePath = new List<DialogueLine>();

        AddToDialogue(serializedDialouge);
        ReadDialogueLine(currentLineIndex);
    }

    public void AddToDialogue(string serializedDialouge)
    {
        List<string> serializedLines = new List<string>(serializedDialouge.Split('\n', StringSplitOptions.RemoveEmptyEntries));

        foreach (string serializedLine in serializedLines)
        {
            DialogueLine dialogueLine = DeserializeLine(serializedLine);
            dialoguePath.Add(dialogueLine);
        }

        SerializedDialoguePath.Add(serializedDialouge);
    }

    public void LoadDialogueFromSave(SaveData saveData)
    {
        StartDialogue(saveData.dialoguePath[0]);

        for (int i = 1; i < saveData.dialoguePath.Count; ++i)
        {
            AddToDialogue(saveData.dialoguePath[i]);
        }
    }
}