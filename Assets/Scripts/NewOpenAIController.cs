using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using UnityEngine;

public class NewOpenAIController : Singleton<NewOpenAIController>
{
    [SerializeField] private string apiKey;
    [SerializeField] private string genre = "romance";
    [SerializeField] private string setting = "high school";
    [SerializeField] private int linesPerScene = 10;
    [SerializeField] private int numberOfCharacters = 5;
    [SerializeField] private string finishedPrompt = "";
    private OpenAIAPI api;
    public Conversation Chat { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        api = new OpenAIAPI(apiKey);
    }

    private void Start()
    {
        GenerateDialogue();
    }

    private async void GenerateDialogue()
    {
        ChatRequest chatRequest = new()
        {
            Model = Model.GPT4Turbo,
            Temperature = 1
        };
        Chat = api.Chat.CreateConversation(chatRequest);
        
        string prompt =
            $"Generate a script for the next scene of a '{genre}' genre visual novel set in the setting '{setting}', consisting of {linesPerScene} lines of dialogue. " +
            $"The story should feature {numberOfCharacters} characters, as well as a narrator named \"Narrator\". Not every character needs to appear in every scene. " +
            "Each line should include the speaking character's name, the text of the dialogue, and the speaker's mood. " +
            "Format the response as a plain JSON object with a top-level key 'DialogueLines'. " +
            "Each entry under 'DialogueLines' should be an object with the keys 'CharacterName', 'DialogueText', and 'Mood'. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        string extractedJson = ExtractJson(assistantResponse);

        Debug.Log(assistantResponse);

        if (extractedJson == null)
        {
            GenerateDialogue();
            return;
        };

        DialogueScene dialogueScene = JsonUtility.FromJson<DialogueScene>(extractedJson);
        foreach (NewDialogueLine dialogueLine in dialogueScene.DialogueLines)
        {
            Debug.Log($"{dialogueLine.CharacterName}: {dialogueLine.DialogueText} <{dialogueLine.Mood}>");
        }
    }

    private string ExtractJson(string rawData)
    {
        int startIndex = rawData.IndexOf('{');
        int endIndex = rawData.LastIndexOf('}');

        if (startIndex == -1 || endIndex == -1 || endIndex < startIndex)
        {
            return null;
        }

        string extractedJson = rawData.Substring(startIndex, endIndex - startIndex + 1);
        return extractedJson;
    }
}
