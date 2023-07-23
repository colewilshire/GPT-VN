using OpenAI_API;
using OpenAI_API.Chat;
using System.Collections.Generic;
using UnityEngine;

public class OpenAIController : Singleton<OpenAIController>
{
    [SerializeField] private string apiKey;
    [SerializeField] private string prompt;
    [SerializeField] private int numberOfLines;
    [SerializeField] private string genre;
    [SerializeField] private string setting;
    [SerializeField] private char stringDelimiter;
    [SerializeField] private List<string> characters;
    private OpenAIAPI api;
    private Conversation chat;

    private void Start()
    {
        api = new OpenAIAPI(apiKey);
        chat = api.Chat.CreateConversation();
        prompt = CreatePrompt();

        chat.AppendSystemMessage(prompt);
        GetResponse();
    }

    private async void GetResponse()
    {
        string response = await chat.GetResponseFromChatbotAsync();
        Debug.Log(response);
    }

    private string CreatePrompt()
    {
        string newPrompt = $"Write a {numberOfLines} line {genre} visual novel script, set in {setting}. Dialogue lines should be outputted in the format CharacterName{stringDelimiter}DialogueText{stringDelimiter}Mood{stringDelimiter}BackgroundScene.";

        if (characters.Count > 0)
        {
            newPrompt += " The cast consists of the characters: ";

            foreach (string characterName in characters)
            {
                newPrompt += $"{characterName}, ";
            }

            newPrompt += ".";
        }

        return newPrompt;
    }
}