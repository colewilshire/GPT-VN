using OpenAI_API.Chat;
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

        //BackgroundController.Instance.SetBackground(currentLineIndex.backgroundImage);
        NameDisplayController.Instance.SetDisplayName(currentLineIndex.characterName);
        TextController.Instance.SetText(currentLineIndex.dialogueText);
        AudioController.Instance.PlaySound(currentLineIndex.voiceLine);
    }

    private void CreateDialogue(string chatbotResponse)
    {
        Debug.Log(chatbotResponse);

        foreach (ChatMessage message in OpenAIController.Instance.Chat.Messages)
        {
            string[] splitMessage = message.Content.Split('\n');

            foreach (string serializedLine in splitMessage)
            {
                DialogueLine dialogueLine = DeserializeLine(serializedLine);
                DialoguePath.Add(dialogueLine);
            }
        }
    }

    private DialogueLine DeserializeLine(string serializedLine)
    {
        string[] splitLine = serializedLine.Split(OpenAIController.Instance.stringDelimiter);
        DialogueLine dialogueLine = DialogueLine.CreateInstance<DialogueLine>();

        dialogueLine.characterName = splitLine[0];
        dialogueLine.dialogueText = splitLine[1];
        dialogueLine.mood = splitLine[2];
        dialogueLine.backgroundName = splitLine[3];

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
        if (!(currentLineIndex - 1 > 0)) return;
        currentLineIndex -= 1;
        ReadDialogueLine(currentLineIndex);
    }

    public void RepeatLine()
    {
        ReadDialogueLine(currentLineIndex);
    }
}