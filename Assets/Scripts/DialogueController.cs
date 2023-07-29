using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    private List<DialogueLine> DialoguePath = new List<DialogueLine>();
    private int currentLineIndex = 0;

    private void Start()
    {
        StartDialogue();
    }

    private async void StartDialogue()
    {
        string chatbotResponse = await OpenAIController.Instance.Chat.GetResponseFromChatbotAsync();

        CreateDialogue(chatbotResponse);
        ReadDialogueLine(currentLineIndex);
    }

    private void ReadDialogueLine(int index)
    {
        DialogueLine currentLineIndex = DialoguePath[index];

        BackgroundController.Instance.SetBackground(currentLineIndex.backgroundImage);//BackgroundController.Instance.GetImageWithTags(new List<ImageTag>() {ImageTag.House, ImageTag.Sky}));
        NameDisplayController.Instance.SetDisplayName(currentLineIndex.characterName);
        TextController.Instance.SetText(currentLineIndex.dialogueText);
        AudioController.Instance.PlaySound(currentLineIndex.voiceLine);
    }

    private void CreateDialogue(string chatbotResponse)
    {
        Debug.Log(chatbotResponse);

        string[] serializedLines = chatbotResponse.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string serializedLine in serializedLines)
        {
            DialogueLine dialogueLine = DeserializeLine(serializedLine);
            DialoguePath.Add(dialogueLine);
        }
    }

    private DialogueLine DeserializeLine(string serializedLine)
    {
        string[] splitLine = serializedLine.Split('|');
        DialogueLine dialogueLine = DialogueLine.CreateInstance<DialogueLine>();

        if (splitLine.Length > 0)
        {
            dialogueLine.characterName = splitLine[0];
        }

        if (splitLine.Length > 1)
        {
            dialogueLine.dialogueText = splitLine[1];
        }

        if (splitLine.Length > 2)
        {
            dialogueLine.mood = splitLine[2];
        }

        if (splitLine.Length > 3)
        {
            if(Enum.TryParse(splitLine[3], true, out BackgroundImageTag tag))
            {
                dialogueLine.backgroundImage = BackgroundController.Instance.GetBackgroundImageWithTags(new List<BackgroundImageTag>() { tag });
            }
            else
            {
                Debug.Log(serializedLine);
            }
        }

        return dialogueLine;
    }

    public void StepForward()
    {
        if (!(DialoguePath.Count > currentLineIndex + 1)) return;
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
        if (!(DialoguePath.Count > 0)) return;
        ReadDialogueLine(currentLineIndex);
    }
}