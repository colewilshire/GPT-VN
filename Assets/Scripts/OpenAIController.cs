using OpenAI_API;
using OpenAI_API.Chat;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenAIController : Singleton<OpenAIController>
{
    [SerializeField] private string apiKey;
    [SerializeField] private string prompt;
    [SerializeField] private int numberOfLines;
    [SerializeField] private string genre;
    [SerializeField] private string setting;
    [SerializeField] private List<string> characters;
    private OpenAIAPI api;
    public Conversation Chat { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        api = new OpenAIAPI(apiKey);
        Chat = api.Chat.CreateConversation();
        prompt = CreatePrompt();

        Chat.AppendSystemMessage(prompt);
    }

    private string CreatePrompt()
    {
        string newPrompt = $"Write a {numberOfLines} line {genre} genre visual novel script, set in {setting}. EEvery line should be delivered in Pipe-Separated Values (PSV) format. Lines must include characterName, dialogueText, mood, and backgroundImageTag, in that order.";

        // Prompt AI with cast of characters
        if (characters.Count > 0)
        {
            newPrompt += " The cast consists of the characters: ";

            foreach (string characterName in characters)
            {
                newPrompt += $"{characterName}, ";
            }

            newPrompt += ".";
        }

        // Prompt AI with available image tags
        newPrompt += " The possible values for BackgroundImageTag are: ";
        BackgroundTag[] tags = (BackgroundTag[]) Enum.GetValues(typeof(BackgroundTag));

        foreach (BackgroundTag tag in tags)
        {
            newPrompt += $"{tag}, ";
        }

        newPrompt += ".";

        return newPrompt;
    }
}