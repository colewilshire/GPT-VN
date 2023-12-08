using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;

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

    public async void CreateNewConversation()
    {
        StateController.Instance.SetStates(GameState.Loading);
        CharacterManager.Instance.ClearCharacters();

        ChatRequest chatRequest = new()
        {
            Model = Model.GPT4Turbo,
            Temperature = 1,
            TopP = 1
        };
        Chat = api.Chat.CreateConversation(chatRequest);

        //
        string castList = await GenerateCastList();
        Dictionary<string, Character> characterDictionary = JsonConvert.DeserializeObject<Dictionary<string, Character>>(castList);

        string initialDialogue = await GenerateInitialDialogue();
        DialogueScene initialDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(initialDialogue);

        //NewDialogueController.Instance.AddSceneToDialogue(initialDialogueScene);

        // string choice = await GenerateChoice();
        // Choice choices = JsonConvert.DeserializeObject<Choice>(choice);

        NewDialogueController.Instance.StartDialogue(initialDialogueScene);
        StateController.Instance.SetStates(GameState.Gameplay);

        //string additionalDialogue = await GenerateAdditionalDialogue();
        //DialogueScene dialogueScene2 = JsonConvert.DeserializeObject<DialogueScene>(additionalDialogue);

        //
        // foreach (KeyValuePair<string, Character> keyValuePair in characterDictionary)
        // {
        //     string characterName = keyValuePair.Key;
        //     Character character = keyValuePair.Value;

        //     Debug.Log($"{characterName}: {character.Accessory}, {character.Eyes}, {character.Hair}, {character.Outfit}");
        // }

        // foreach (NewDialogueLine dialogueLine in dialogueScene.DialogueLines)
        // {
        //      Debug.Log($"{dialogueLine.CharacterName}: {dialogueLine.DialogueText} <{dialogueLine.Mood}>");
        // }

        // foreach (NewDialogueLine dialogueLine in choices.Choices)
        // {
        //      Debug.Log($"{dialogueLine.CharacterName}: {dialogueLine.DialogueText} <{dialogueLine.Mood}>");
        // }

        // foreach (NewDialogueLine dialogueLine in dialogueScene2.DialogueLines)
        // {
        //      Debug.Log($"{dialogueLine.CharacterName}: {dialogueLine.DialogueText} <{dialogueLine.Mood}>");
        // }
    }

    private async Task<string> GenerateCastList()
    {
        string prompt =
            $"Generate a cast list of {numberOfCharacters} characters for the next scene of a '{genre}' genre visual novel set in the setting '{setting}'. " +
            "One of the characters should be named 'Main Character', and serve as the protagonist of the story. " +
            "One of the characters should be named 'Narrator', and serve as the story's narrator. " +
            $"The other {numberOfCharacters - 2} characters are up to you to create. Characters other than 'Main Character' and 'Narrator' should have actual human names for their name, not titles. " +
            "Each character in the list should have a name, hair style, face/hair accessory, outfit, and eye color. " +
            "Format the response as a plain JSON object with only the characters' names as top-level keys'. " +
            "Each entry under a top-level key should be an object with the keys 'Hair', 'Outfit', 'Accessory', and 'Eyes'. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        string extractedJson = ExtractJson(assistantResponse);

        Debug.Log(assistantResponse);

        if (extractedJson == null)
        {
            Debug.Log("Error generating usable response. Retrying.");
            return await GenerateCastList();
        };

        return extractedJson;
    }

    private async Task<string> GenerateInitialDialogue()
    { 
        string prompt =
            $"Generate a script for the next scene of a '{genre}' genre visual novel set in the setting '{setting}', consisting of {linesPerScene} lines of dialogue. " +
            "Only a few characters from the cast list should appear in every scene. Some characters should be rarely appearing side characters, and the Main Character and Narrator should appear frequently." +
            "The cast of the story should consist of characters from the previously generated cast list." +
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
            Debug.Log("Error generating usable response. Retrying.");
            return await GenerateInitialDialogue();
        };

        return extractedJson;
    }

    private async Task<string> GenerateChoice()
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

        Debug.Log(assistantResponse);

        if (extractedJson == null)
        {
            Debug.Log("Error generating usable response. Retrying.");
            return await GenerateInitialDialogue();
        };

        return extractedJson;
    }

    private async Task<string> GenerateAdditionalDialogue(string castList = "")
    { 
        string prompt =
            $"From where the story last left off, continue the visual novel's script with {linesPerScene} more lines of dialogue. ";

        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        string extractedJson = ExtractJson(assistantResponse);

        Debug.Log(assistantResponse);

        if (extractedJson == null)
        {
            Debug.Log("Error generating usable response. Retrying.");
            return await GenerateAdditionalDialogue();
        };

        return extractedJson;
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
