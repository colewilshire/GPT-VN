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

        BackgroundController.Instance.SetBackground(currentLineIndex.backgroundImage);
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
            dialogueLine.characterName = splitLine[0].Trim();
        }

        if (splitLine.Length > 1)
        {
            dialogueLine.dialogueText = splitLine[1].Trim('"');
        }

        if (splitLine.Length > 2)
        {
            if(Enum.TryParse(splitLine[2], true, out BackgroundTag tag))
            {
                List<BackgroundTag> backgroundTags = new List<BackgroundTag>() { tag };
                dialogueLine.backgroundImage = BackgroundController.Instance.GetBackgroundImageWithTags(backgroundTags).image;
            }
        }

        if (splitLine.Length > 3)
        {
            dialogueLine.mood = splitLine[3];
        }

        if (splitLine.Length > 7)
        {
            List<HairTag> hairTags = new List<HairTag>();
            List<OutfitTag> outfitTags = new List<OutfitTag>();
            List<FaceTag> faceTags = new List<FaceTag>();
            List<AccessoryTag> accessoryTags = new List<AccessoryTag>();

            if(Enum.TryParse(splitLine[4], true, out HairTag hairTag))
            {
                hairTags.Add(hairTag);
            }

            if(Enum.TryParse(splitLine[5], true, out OutfitTag outfitTag))
            {
                outfitTags.Add(outfitTag);
            }

            if(Enum.TryParse(splitLine[6], true, out FaceTag faceTag))
            {
                faceTags.Add(faceTag);
            }

            if(Enum.TryParse(splitLine[7], true, out AccessoryTag accessoryTag))
            {
                accessoryTags.Add(accessoryTag);
            }

            CharacterGenerationController.Instance.GenerateCharacterPortrait(dialogueLine.characterName, accessoryTags, hairTags, outfitTags, faceTags);
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