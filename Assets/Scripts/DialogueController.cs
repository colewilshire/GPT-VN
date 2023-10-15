using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    private List<DialogueLine> dialoguePath = new List<DialogueLine>();
    [SerializeField] ContinueStoryButton continueStoryButton;
    public int CurrentLineIndex {get; private set;} = 0;
    public string SerializedDialoguePath {get; private set;} = "";

    private async void ReadDialogueLine(int index)
    {
        DialogueLine currentLine = dialoguePath[index];

        UIEffectController.Instance.TerminateEffects();
        BackgroundController.Instance.SetBackground(currentLine.BackgroundImage);
        CharacterManager.Instance.ShowPortrait(currentLine.CharacterName, currentLine.Mood);
        NameDisplayController.Instance.SetDisplayName(currentLine.CharacterName);
        await TextController.Instance.SetText(currentLine.DialogueText);
        AudioController.Instance.PlaySound(currentLine.VoiceLine);
    }

    private DialogueLine DeserializeLine(string serializedLine)
    {
        List<string> splitLine = new List<string>(serializedLine.Split('|', StringSplitOptions.RemoveEmptyEntries))
            .Select(str => str.Trim())
            .Select(str => str.Trim('"'))
            .ToList();
        DialogueLine dialogueLine = DialogueLine.CreateInstance<DialogueLine>();
        string characterName = "";
        string dialogueText = "";
        string characterMood = "";
        string backgroundName = "";

        if (splitLine.Count > 3)
        {
            characterName = splitLine[0];
            dialogueText = splitLine[1];
            characterMood = splitLine[2];
            backgroundName = splitLine[3];
        }
        else if (splitLine.Count > 0)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                ["characterName"] = "",
                ["dialogueText"] = "",
                ["characterMood"] = "",
                ["backgroundName"] = "",
            };

            foreach (string unknownString in splitLine)
            {
                if (CharacterManager.Instance.Characters.ContainsKey(unknownString))
                {
                    Debug.Log($"Name: {unknownString}");
                    dict["characterName"] = unknownString;
                }
                else if (BackgroundController.Instance.GetBackgroundImageFromName(unknownString) != null)
                {
                    Debug.Log($"Background: {unknownString}");
                    backgroundName = unknownString;
                }
                else
                {
                    if (unknownString.Length > dict["dialogueText"].Length)
                    {
                        Debug.Log($"Dialogue: {unknownString}");
                        dict["characterMood"] = dict["dialogueText"];
                        dict["dialogueText"] = unknownString;
                    }
                    else
                    {
                        Debug.Log($"Mood: {unknownString}");
                        dict["characterMood"] = unknownString; 
                    }
                }
            }

            characterName = dict["characterName"];
            dialogueText = dict["dialogueText"];
            characterMood = dict["characterMood"];
            backgroundName = dict["backgroundName"];
            // Now add some functionality to rewrite the Chat log to make it appear to the AI that it did everything correct and hopefully train it
        }

        dialogueLine.CharacterName = characterName;
        dialogueLine.DialogueText = dialogueText;
        dialogueLine.Mood = characterMood;
        dialogueLine.BackgroundImage = BackgroundController.Instance.GetBackgroundImageFromName(backgroundName);

        return dialogueLine;
    }

    private void AddToDialogue(string serializedDialogue)
    {
        List<string> serializedLines = new List<string>(serializedDialogue.Split('\n', StringSplitOptions.RemoveEmptyEntries));

        foreach (string serializedLine in serializedLines)
        {
            DialogueLine dialogueLine = DeserializeLine(serializedLine);
            dialoguePath.Add(dialogueLine);
        }

        SerializedDialoguePath += $"{serializedDialogue}\n";
    }

    public void StepForward()
    {
        if (!(dialoguePath.Count > CurrentLineIndex + 1))
        {
           continueStoryButton.ShowButton();
           return;
        }

        CurrentLineIndex += 1;
        ReadDialogueLine(CurrentLineIndex);
        SaveController.Instance.Autosave(CurrentLineIndex);
    }

    public void StepBackward()
    {
        if (!(CurrentLineIndex - 1 >= 0)) return;
        continueStoryButton.HideButton();
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

    public void ContinueDialogue(string serializedDialogue)
    {
        ++CurrentLineIndex;

        AddToDialogue(serializedDialogue);
        ReadDialogueLine(CurrentLineIndex);
        SaveController.Instance.Autosave(CurrentLineIndex);
    }

    public void LoadDialogueFromSave(SaveData saveData)
    {
        StartDialogue(saveData.DialoguePath, saveData.CurrentLineIndex);
    }
}