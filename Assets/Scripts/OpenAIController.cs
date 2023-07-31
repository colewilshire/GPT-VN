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
        Type backgroundTag = typeof(BackgroundTag);
        Type mood = typeof(Mood);
        Type hairTag = typeof(HairTag);
        Type outfitTag = typeof(OutfitTag);
        Type faceTag = typeof(FaceTag);
        Type accessoryTag = typeof(AccessoryTag);
        string newPrompt = $"Write a {numberOfLines} line {genre} genre visual novel script, set in {setting}. Every line should be delivered in Pipe-Separated Values (PSV) format. Lines must include characterName, dialogueText, {backgroundTag}, {mood}, {hairTag}, {outfitTag}, {faceTag}, and {accessoryTag} in that order.";

        newPrompt += PromptWithCharacters();
        newPrompt += PromptWithEnum(backgroundTag);
        newPrompt += PromptWithEnum(mood);
        newPrompt += PromptWithEnum(hairTag);
        newPrompt += PromptWithEnum(outfitTag);
        newPrompt += PromptWithEnum(faceTag);
        newPrompt += PromptWithEnum(accessoryTag);

        return newPrompt;
    }

    private string PromptWithCharacters()
    {
        string newPrompt = "";

        if (characters.Count > 0)
        {
            newPrompt += " The cast consists of the characters: ";
            newPrompt += string.Join(", ", characters);

            newPrompt += ".";
        }

        return newPrompt;
    }

    private string PromptWithEnum(Type enumType)
    {
        string enumName = enumType.Name;
        string newPrompt = $" The only possible values for {enumName} are: ";
        Array values = Enum.GetValues(enumType);
        List<string> valueList = new List<string>();

        foreach (var value in values)
        {
            valueList.Add(value.ToString());
        }

        newPrompt += string.Join(", ", valueList);
        newPrompt += ".";

        return newPrompt;
    }
}