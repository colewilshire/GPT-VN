using OpenAI_API;
using OpenAI_API.Chat;
using System.Collections.Generic;
using UnityEngine;

public class OpenAIController : Singleton<OpenAIController>
{
    private OpenAIAPI api;
    private List<ChatMessage> messages;
    private Conversation chat;
    private string prompt = "Write a 5 line play set in a fantasy setting. The characters are a young man and young woman, as well as a third person omnipotent narrator.";
    private string apiKey = "";

    private void Start()
    {
        api = new OpenAIAPI(apiKey);
        chat = api.Chat.CreateConversation();

        chat.AppendSystemMessage(prompt);
        GetResponse();
    }

    private async void GetResponse()
    {
        string response = await chat.GetResponseFromChatbotAsync();
        Debug.Log(response);
    }
}