using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;

public class NewOpenAIController : Singleton<NewOpenAIController>
{
    [Header("API")]
    [SerializeField] private string apiKey;

    [Header("Story Settings")]
    [SerializeField] private int linesPerScene = 10;
    [SerializeField] private int numberOfCharacters = 5;

    [Header("Debug")]
    [SerializeField] private string finishedPrompt = "";

    private OpenAIAPI api;
    public string Genre { get; private set; }
    public string Setting { get; private set; }
    public Conversation Chat { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        api = new OpenAIAPI(apiKey);
    }

    public async void CreateNewConversation()
    {
        // Get user input for genre and setting
        Genre = await InputPrompt.Instance.PromptInput("enter a genre", "romance");
        if (Genre == null) return;

        Setting = await InputPrompt.Instance.PromptInput("enter a setting", "high school");
        if (Setting == null) return;

        // Start loading
        StateController.Instance.SetStates(GameState.Loading);
        CharacterManager.Instance.ClearCharacters();

        ChatRequest chatRequest = new()
        {
            Model = Model.GPT4o,
            Temperature = 1,
            MaxTokens = 4096
        };
        Chat = api.Chat.CreateConversation(chatRequest);

        // Make chat requests
        Dictionary<string, CharacterDescription> castList = await GenerateCastList();
        DialogueScene initialDialogueScene = await GenerateInitialDialogue();

        // Verify requests are good
        if (castList == null || initialDialogueScene == null)
        {
            StateController.Instance.SetStates(GameState.MainMenu);
            return;
        }

        // Interpret chat requests
        NewCharacterManager.Instance.GenerateCharacterPortraits(castList);
        NewDialogueController.Instance.StartDialogue(initialDialogueScene);
        NewSaveController.Instance.Save(Random.Range(0, 10000).ToString());
        StateController.Instance.SetStates(GameState.Gameplay);
    }

    public void LoadConversationFromSave(string saveName)
    {
        NewSaveData saveData = NewSaveController.Instance.LoadSave(saveName);
        if (saveData == null) return;

        StateController.Instance.SetStates(GameState.Loading);
        CharacterManager.Instance.ClearCharacters();

        ChatRequest chatRequest = new()
        {
            Model = Model.GPT4o,
            Temperature = 1,
            MaxTokens = 4096
        };

        Chat = api.Chat.CreateConversation(chatRequest);

        foreach(ChatMessage message in saveData.Messages)
        {
            if (message.Role == ChatMessageRole.System)
            {
                Chat.AppendSystemMessage(message.Content);
            }
            else if (message.Role == ChatMessageRole.Assistant)
            {
                Chat.AppendExampleChatbotOutput(message.Content);
            }
            else
            {
                Chat.AppendUserInput(message.Content);
            }
        }

        Genre = saveData.Genre;
        Setting = saveData.Setting;

        NewCharacterManager.Instance.GenerateCharacterPortraits(saveData.CharacterDescriptions);
        StateController.Instance.SetStates(GameState.Gameplay);
        NewDialogueController.Instance.StartDialogue(new DialogueScene() { DialogueLines = saveData.DialoguePath }, saveData.CurrentLineIndex);
    }

    private async Task<Dictionary<string, CharacterDescription>> GenerateCastList()
    {
        string prompt =
            $"Generate a cast list of {numberOfCharacters} characters for the next scene of a '{Genre}' genre visual novel set in the setting '{Setting}'. " +
            "One of the characters should be named 'Main Character', and serve as the protagonist of the story. " +
            "One of the characters should be named 'Narrator', and serve as the story's narrator. " +
            $"The other {numberOfCharacters - 2} characters are up to you to create. Characters other than 'Main Character' and 'Narrator' should have actual human names for their name, not titles. " +
            "Each character in the list should have a name, body type, hair style, face/hair accessory, outfit, and eye color. " +
            $"Body types must be chosen from the following list: 'none', 'feminine', 'masculine'. " +
            $"Accessories must be chosen from the following list: {NewCharacterManager.Instance.ListAccessories()}. " +
            $"Hairs must be chosen from the following list: {NewCharacterManager.Instance.ListHairs()}. " +
            $"Outfits must be chosen from the following list: {NewCharacterManager.Instance.ListOutfits()}. " +
            "Chosen outfits and accessories should be appropriate for the story's setting, if possible. For example, in a uniformed setting, all characters of the same geneder and position should be wearing the same uniform. " +
            $"Eye colors must be chosen from the following list: {NewCharacterManager.Instance.ListFaces()}. " +
            "Format the response as a plain JSON object with only the characters' names as top-level keys'. " +
            "Each entry under a top-level key should be an object with the keys 'BodyType', 'Hair', 'Outfit', 'Accessory', and 'Eyes'. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        string extractedJson = ExtractJson(assistantResponse);
        Dictionary<string, CharacterDescription> characterDescriptions;

        Debug.Log(assistantResponse);

        try
        {
            characterDescriptions = JsonConvert.DeserializeObject<Dictionary<string, CharacterDescription>>(extractedJson);
        }
        catch
        {
            Debug.Log("Error generating usable response.");
            return null;
        }

        return characterDescriptions;
    }

    private async Task<DialogueScene> GenerateInitialDialogue()
    {
        string prompt =
            $"Generate a script for the next scene of a '{Genre}' genre visual novel set in the setting '{Setting}', consisting of {linesPerScene} lines of dialogue. " +
            "Only a few characters from the cast list should appear in every scene. Some characters should be rarely appearing side characters, and the Main Character and Narrator should appear frequently. " +
            "The cast of the story should consist of characters from the previously generated cast list. " +
            "Each line should include the speaking character's name, the text of the dialogue, the speaker's mood, and the background image to be displayed. " +
            "Format the response as a plain JSON object with a top-level key 'DialogueLines'. " +
            "Each entry under 'DialogueLines' should be an object with the keys 'CharacterName', 'DialogueText', 'Mood', and 'BackgroundDescription'." +
            $"BackgroundsDescriptions should be chosen from the following list: {NewBackgroundController.Instance.ListBackgrounds()}. " +
            "Unless the story calls for a change in location, the BackgroundDescription should not change from one line of dialogue to the next. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        DialogueScene initialDialogueScene;
        string extractedJson;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        extractedJson = ExtractJson(assistantResponse);

        Debug.Log(assistantResponse);

        try
        {
            initialDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(extractedJson);
        }
        catch
        {
            Debug.Log("Error generating usable response.");
            return null;
        }

        initialDialogueScene.DialogueLines.Add(new()
        {
            CharacterName = "Main Character",
            DialogueText = "What should I choose?",
            Choice = await GenerateChoice()
        });

        return initialDialogueScene;
    }

    private async Task<Choice> GenerateChoice()
    { 
        string prompt =
            "From where the story left off, offer the player 3 choices of dialogue lines for the Main Character to choose. " +
            "This choice should impact the trajectory of the story. " +
            "Each line should include the speaking character's name, the text of the dialogue, and the speaker's mood. " +
            "Format the response as a plain JSON object with a top-level key 'Choices'. " +
            "Each entry under 'Choices' should be an object with the keys 'CharacterName', 'DialogueText', and 'Mood'. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        string extractedJson = ExtractJson(assistantResponse);
        Choice choice = JsonConvert.DeserializeObject<Choice>(extractedJson);

        Debug.Log(assistantResponse);

        if (choice == null)
        {
            Debug.Log("Error generating usable response. Retrying.");
            return await GenerateChoice();
        }

        return choice;
    }

    public async Task<DialogueScene> GenerateAdditionalDialogue(string choiceText = null)
    {
        string prompt = "";

        if (choiceText != null)
        {
            prompt += $"The player chose the dialogue option \"{choiceText}\". ";
        }

        prompt +=
            $"From where the story last left off, continue the visual novel's script with {linesPerScene} more lines of dialogue. ";

        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        string extractedJson = ExtractJson(assistantResponse);

        DialogueScene newDialogueScene;

        Debug.Log(assistantResponse);

        try
        {
            newDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(extractedJson);
        }
        catch
        {
            Debug.Log("Error generating usable response.");
            return null;
        }

        newDialogueScene.DialogueLines.Add(new()
        {
            CharacterName = "Main Character",
            DialogueText = "What should I choose?",
            Choice = await GenerateChoice()
        });

        return newDialogueScene;
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
