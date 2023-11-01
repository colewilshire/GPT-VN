using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    private readonly char choiceCharacter = '~';
    private List<DialogueLine> dialoguePath = new();
    public int CurrentLineIndex {get; private set;} = 0;
    public string SerializedDialoguePath {get; private set;} = "";

    private void ReadDialogueLine(int index)
    {
        DialogueLine currentLine = dialoguePath[index];

        UIEffectController.Instance.TerminateEffects();
        BackgroundController.Instance.SetBackground(currentLine.BackgroundImage);
        CharacterPortrait characterPortrait = CharacterManager.Instance.ShowPortrait(currentLine.CharacterName, currentLine.Mood);

        if (characterPortrait)
        {
            NameDisplayController.Instance.SetDisplayName(characterPortrait.DisplayName);
        }
        else
        {
            NameDisplayController.Instance.SetDisplayName("");
        }

        TextController.Instance.SetText(currentLine.DialogueText);
        AudioController.Instance.PlaySound(currentLine.VoiceLine);

        if (currentLine.DialogueChoice)
        {
            TextController.Instance.SetText("What should I chooose?");
            DialogueChoiceController.Instance.ShowChoices(currentLine.DialogueChoice);
        }
    }

    public DialogueLine DeserializeLine(string serializedLine)
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
            Dictionary<string, string> dict = new()
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
            Match match = Regex.Match(dict["dialogueText"], @"""([^""]*)""");
            if (match.Success)
            {
                dialogueText = match.Groups[1].Value;
            }
            characterMood = dict["characterMood"];
            backgroundName = dict["backgroundName"];
            // Now add some functionality to rewrite the Chat log to make it appear to the AI that it did everything correct and hopefully train it
        }

        dialogueLine.CharacterName = characterName;
        dialogueLine.DialogueText = dialogueText;
        dialogueLine.Mood = Thesaurus.Instance.GetMoodSynonym(characterMood);
        dialogueLine.BackgroundImage = BackgroundController.Instance.GetBackgroundImageFromName(backgroundName);
        dialogueLine.SerializedLine = serializedLine;

        return dialogueLine;
    }

    public void AddToDialogue(string serializedDialogue, bool isNewGame = false)
    {
        List<string> serializedLines = new(serializedDialogue.Split('\n', StringSplitOptions.RemoveEmptyEntries));
        DialogueLine firstChoice = null;

        foreach (string serializedLine in serializedLines)
        {
            bool isDialogueChoice = serializedLine.StartsWith(choiceCharacter);
            DialogueLine dialogueLine;

            if (isDialogueChoice && !firstChoice)
            {
                firstChoice = DeserializeLine(serializedLine.TrimStart(choiceCharacter));
                firstChoice.DialogueChoice = DialogueChoice.CreateInstance<DialogueChoice>();

                firstChoice.DialogueChoice.Choices.Add(firstChoice);
                dialoguePath.Add(firstChoice);

                // Chances are, the character making decisions is intended by the AI to be the main character
                if (firstChoice.CharacterName != null && CharacterManager.Instance.MainCharacter == null && isNewGame)
                {
                    CharacterManager.Instance.SetMainCharacter(firstChoice.CharacterName);
                }
            }
            else if (isDialogueChoice && firstChoice)
            {
                firstChoice.DialogueChoice.Choices.Add(DeserializeLine(serializedLine.TrimStart(choiceCharacter)));
            }
            else
            {
                dialogueLine = DeserializeLine(serializedLine);
                firstChoice = null;

                dialoguePath.Add(dialogueLine);
            }
        }

        SerializedDialoguePath += $"{serializedDialogue}";

        if (!SerializedDialoguePath.EndsWith('\n'))
        {
            SerializedDialoguePath += '\n';
        }
    }

    public void AddChoiceToDialogue(string serializedDialogue)
    {
        serializedDialogue = $"~{serializedDialogue.Trim('\n').Replace("\n", $"\n{choiceCharacter}")}";
        AddToDialogue(serializedDialogue, true);
    }

    public void StepForward()
    {
        if (!(dialoguePath.Count > CurrentLineIndex + 1)) return;

        CurrentLineIndex += 1;
        StateController.Instance.SetSubmenuState(GameState.Gameplay);
        SaveController.Instance.Autosave(CurrentLineIndex);
        ReadDialogueLine(CurrentLineIndex);
    }

    public void StepBackward()
    {
        if (!(CurrentLineIndex - 1 >= 0)) return;

        CurrentLineIndex -= 1;
        StateController.Instance.SetSubmenuState(GameState.Gameplay);
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