using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    private List<DialogueLine> dialoguePath = new List<DialogueLine>();
    public int CurrentLineIndex {get; private set;} = 0;
    public string SerializedDialoguePath {get; private set;} = "";

    private void ReadDialogueLine(int index)
    {
        DialogueLine currentLine = dialoguePath[index];

        if (currentLine.DialogueChoice)
        {
            ChoiceController.Instance.ShowChoices(currentLine.DialogueChoice);
            return;
        }

        UIEffectController.Instance.TerminateEffects();
        BackgroundController.Instance.SetBackground(currentLine.BackgroundImage);
        CharacterManager.Instance.ShowPortrait(currentLine.CharacterName, currentLine.Mood);
        NameDisplayController.Instance.SetDisplayName(currentLine.CharacterName);
        TextController.Instance.SetText(currentLine.DialogueText);
        AudioController.Instance.PlaySound(currentLine.VoiceLine);
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
        dialogueLine.Mood = characterMood;
        dialogueLine.BackgroundImage = BackgroundController.Instance.GetBackgroundImageFromName(backgroundName);

        return dialogueLine;
    }

    private void AddToDialogue(string serializedDialogue)
    {
        //Debug.Log(serializedDialogue[0]);

        //if (serializedDialogue.StartsWith('~'))

        // ^ Here lies the problem: when new, AddToDialogue is called twice on two different serializedDialogues, whereas there is only one combined on loading a save,
        // which doesn't start with ~, since choices are never presented before the first on-rails dialogue segment
        // Thus, deserialize line is probably the ideal place to do things

        //{
            //Debug.Log($"qqq: {serializedDialogue}");
            //AddChoiceToDialogue(serializedDialogue);
            //return;
        //}

        List<string> serializedLines = new(serializedDialogue.Split('\n', StringSplitOptions.RemoveEmptyEntries));
        DialogueLine firstChoice = null;

        foreach (string serializedLine in serializedLines)
        {
            //Debug.Log(serializedLine);

            if (!firstChoice && serializedLine.StartsWith('~'))
            {
                Debug.Log("A: " + serializedLine);
                
                firstChoice = DeserializeLine(serializedLine.TrimStart('~'));
                firstChoice.DialogueChoice = DialogueChoice.CreateInstance<DialogueChoice>();
                firstChoice.DialogueChoice.Choices.Add(firstChoice);

                dialoguePath.Add(firstChoice);
            }
            else if (firstChoice && serializedLine.StartsWith('~'))
            {
                Debug.Log("B: " + serializedLine);

                firstChoice.DialogueChoice.Choices.Add(DeserializeLine(serializedLine.TrimStart('~')));
            }
            else
            {
                DialogueLine dialogueLine = DeserializeLine(serializedLine);
                dialoguePath.Add(dialogueLine);
            }
        }
        ////Debug.Log($"{serializedDialogue}");
        SerializedDialoguePath += $"{serializedDialogue}";

        if (!SerializedDialoguePath.EndsWith('\n'))
        {
            SerializedDialoguePath += '\n';
        }
    }

    public void AddChoiceToDialogue(string serializedDialogue)
    {
        Debug.Log($"Original: {serializedDialogue}");

        // Main Character|We should invite Aiko and Yumi to join the basketball team too. What do you think?|Excited|School Rooftop
        // Main Character|Do you have any tips for beginners like us?|Curious|School Gymnasium
        // Main Character|I'm a bit nervous about joining the team. Can you help boost my confidence?|Nervous|School Classroom

        // ~Main Character|We should invite Aiko and Yumi to join the basketball team too. What do you think?|Excited|School Rooftop~Main Character|Do you have any tips for
        // beginners like us?|Curious|School Gymnasium~Main Character|I'm a bit nervous about joining the team. Can you help boost my confidence?|Nervous|School Classroom

        //serializedDialogue = $"~{serializedDialogue.Trim('\n').Replace("\n", "~\n")}\n";
        serializedDialogue = $"~{serializedDialogue.Trim('\n').Replace("\n", "\n~")}";
        //serializedDialogue = $"~{serializedDialogue}\r\n";

        Debug.Log($"Modified: {serializedDialogue}");

        //return;
        AddToDialogue(serializedDialogue);
    }

    // private void AddChoiceToDialogue(string serializedDialogue)
    // {
    //     List<string> serializedLines = new(serializedDialogue.Split('~', StringSplitOptions.RemoveEmptyEntries));
    //     List<DialogueLine> dialogueLines = new();
    //     DialogueChoice dialogueChoice = DialogueChoice.CreateInstance<DialogueChoice>();

    //     foreach (string serializedLine in serializedLines)
    //     {
    //         //Debug.Log($"www: {serializedLine}");
    //         DialogueLine dialogueLine = DeserializeLine(serializedLine);
    //         dialogueLine.SerializedLine = serializedLine;

    //         dialogueChoice.Choices.Add(dialogueLine);
    //         dialogueLines.Add(dialogueLine);
    //     }

    //     foreach (DialogueLine dialogueLine in dialogueLines)
    //     {
    //         dialogueLine.DialogueChoice = dialogueChoice;
    //         dialoguePath.Add(dialogueLine);
    //     }

    //     //Debug.Log($"zzz: {serializedDialogue}");
    //     SerializedDialoguePath += $"{serializedDialogue}";

    //     if (!SerializedDialoguePath.EndsWith('\n'))
    //     {
    //         SerializedDialoguePath += '\n';
    //     }
    // }

    public async void StepForward()
    {
        if (!(dialoguePath.Count > CurrentLineIndex + 1))
        {
            bool boolean = await ConfirmationPrompt.Instance.PromptConfirmation("continue story?");
            if (boolean)
            {
                OpenAIController.Instance.GenerateAdditionalDialogue();
            }

            return;
        }

        CurrentLineIndex += 1;
        ReadDialogueLine(CurrentLineIndex);
        SaveController.Instance.Autosave(CurrentLineIndex);
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
        //Debug.Log($"{serializedDialogue}");

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