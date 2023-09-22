using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    private List<DialogueLine> dialoguePath = new List<DialogueLine>();
    public int CurrentLineIndex {get; private set;} = 0;
    public string SerializedDialoguePath = "";

    private void ReadDialogueLine(int index)
    {
        DialogueLine currentLine = dialoguePath[index];

        BackgroundController.Instance.SetBackground(currentLine.BackgroundImage);
        CharacterManager.Instance.ShowPortait(currentLine.CharacterName, currentLine.FacialExpression);
        NameDisplayController.Instance.SetDisplayName(currentLine.CharacterName);
        TextController.Instance.SetText(currentLine.DialogueText);
        AudioController.Instance.PlaySound(currentLine.VoiceLine);
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
        string characterExpression = splitLine[2];
        string backgroundName = splitLine[3];

        dialogueLine.CharacterName = characterName;
        dialogueLine.DialogueText = dialogueText;
        dialogueLine.FacialExpression = characterExpression;
        dialogueLine.BackgroundImage = BackgroundController.Instance.GetBackgroundImageFromName(backgroundName);

        return dialogueLine;
    }

    public void StepForward()
    {
        if (!(dialoguePath.Count > CurrentLineIndex + 1)) return;
        CurrentLineIndex += 1;
        ReadDialogueLine(CurrentLineIndex);
    }

    public void StepBackward()
    {
        if (!(CurrentLineIndex - 1 >= 0)) return;
        CurrentLineIndex -= 1;
        ReadDialogueLine(CurrentLineIndex);
    }

    public void RepeatLine()
    {
        if (!(dialoguePath.Count > 0)) return;
        ReadDialogueLine(CurrentLineIndex);
    }

    public void StartDialogue(string serializedDialogue, int startingLineIndex = 0)
    {
        CurrentLineIndex = startingLineIndex;
        dialoguePath = new List<DialogueLine>();
        SerializedDialoguePath = "";

        AddToDialogue(serializedDialogue);
        ReadDialogueLine(CurrentLineIndex);
    }

    public void AddToDialogue(string serializedDialogue)
    {
        List<string> serializedLines = new List<string>(serializedDialogue.Split('\n', StringSplitOptions.RemoveEmptyEntries));

        foreach (string serializedLine in serializedLines)
        {
            DialogueLine dialogueLine = DeserializeLine(serializedLine);
            dialoguePath.Add(dialogueLine);
        }

        SerializedDialoguePath += $"{serializedDialogue}\n";
    }

    public void LoadDialogueFromSave(SaveData saveData)
    {
        StartDialogue(saveData.DialoguePath, saveData.CurrentLineIndex);
    }
}