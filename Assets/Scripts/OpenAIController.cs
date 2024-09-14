using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OpenAI.Chat;
using Newtonsoft.Json;
using UnityEngine;

public class OpenAIController : Singleton<OpenAIController>
{
    [Header("API")]
    [SerializeField] private string apiKey;

    [Header("Story Settings")]
    [SerializeField] private int linesPerScene = 10;
    [SerializeField] private int numberOfCharacters = 5;

    [Header("Debug")]
    [SerializeField] private string finishedPrompt = "";

    private ChatClient client;
    private List<ChatMessage> Messages;

    public string Genre { get; private set; }
    public string Setting { get; private set; }
    public string ProtagonistName { get; private set; }
    public List<KeyValuePair<ChatMessageRole, string>> MessageDictionary;

    protected override void Awake()
    {
        base.Awake();

        client = new ChatClient("gpt-4o", apiKey);
    }

    public async void CreateNewConversation()
    {
        // Illegal characters for Windows filenames
        string illegalFilenameCharacters = @"[<>:""/\\|?*]";

        // Get user input for genre and setting
        Genre = await InputPrompt.Instance.PromptInput("enter a genre", "romance");
        if (Genre == null) return;
        Genre = Regex.Replace(Genre, illegalFilenameCharacters, "");

        Setting = await InputPrompt.Instance.PromptInput("enter a setting", "high school");
        if (Setting == null) return;
        Setting = Regex.Replace(Setting, illegalFilenameCharacters, "");

        ProtagonistName = await InputPrompt.Instance.PromptInput("name your character", "protagonist");
        if (ProtagonistName == null) return;
        ProtagonistName = Regex.Replace(ProtagonistName, illegalFilenameCharacters, "");

        // Start loading
        StateController.Instance.SetStates(GameState.Loading);

        Messages = new();
        MessageDictionary = new();

        // Make chat requests
        LoadingScreen.Instance.StartLoading(LoadingState.Initial);
        Dictionary<string, CharacterDescription> castList = await GenerateCastList();
        DialogueScene initialDialogueScene = await GenerateInitialDialogue();

        // Verify requests are good
        if (castList == null || initialDialogueScene == null)
        {
            StateController.Instance.SetStates(GameState.MainMenu);
            return;
        }

        // Overwrite protagonist appearance with custom appearance
        foreach (KeyValuePair<string, CharacterDescription> kvp in castList)
        {
            if (kvp.Key.ToLower() == ProtagonistName.ToLower())
            {
                castList[kvp.Key] = new()
                {
                    BodyType = "feminine",
                    Hair = CharacterCreationController.Instance.MainCharacterPortait.Appearance.Hair.Description,
                    Outfit = CharacterCreationController.Instance.MainCharacterPortait.Appearance.Outfit.Description,
                    Accessory = CharacterCreationController.Instance.MainCharacterPortait.Appearance.Accessory.Description,
                    Eyes = CharacterCreationController.Instance.MainCharacterPortait.Appearance.Face.Description
                };

                break;
            }
        }

        // Interpret chat requests
        CharacterManager.Instance.GenerateCharacterPortraits(castList);
        DialogueController.Instance.StartDialogue(initialDialogueScene);
        SaveController.Instance.Save($"{ProtagonistName} - {Setting} {Genre}");
        StateController.Instance.SetStates(GameState.Gameplay);
    }

    public void LoadConversationFromSave(string saveName)
    {
        SaveData saveData = SaveController.Instance.LoadSave(saveName);
        if (saveData == null) return;

        StateController.Instance.SetStates(GameState.Loading);

        Messages = new();
        MessageDictionary = new();

        foreach(KeyValuePair<ChatMessageRole, string> kvp in saveData.Messages)
        {
            if (kvp.Key == ChatMessageRole.System)
            {
                Messages.Add(new SystemChatMessage(kvp.Value));
            }
            else if (kvp.Key == ChatMessageRole.Assistant)
            {
                Messages.Add(new AssistantChatMessage(kvp.Value));
            }
            else
            {
                Messages.Add(new UserChatMessage(kvp.Value));
            }
        }

        Genre = saveData.Genre;
        Setting = saveData.Setting;
        ProtagonistName = saveData.ProtagonistName;

        CharacterManager.Instance.GenerateCharacterPortraits(saveData.CharacterDescriptions);
        StateController.Instance.SetStates(GameState.Gameplay);
        DialogueController.Instance.StartDialogue(new DialogueScene() { DialogueLines = saveData.DialoguePath }, saveData.CurrentLineIndex);
    }

    private async Task<Dictionary<string, CharacterDescription>> GenerateCastList()
    {
        string prompt =
            $"Generate a cast list of {numberOfCharacters} characters for the next scene of a '{Genre}' genre visual novel set in the setting '{Setting}'. " +
            $"One of the characters should be named '{ProtagonistName}', and serve as the protagonist of the story. " +
            "One of the characters should be named 'Narrator', and serve as the story's narrator. " +
            $"The other {numberOfCharacters - 2} characters are up to you to create. Characters other than 'Narrator' should have actual human names for their name, not titles. " +
            "Each character in the list should have a name, body type, hair style, face/hair accessory, outfit, and eye color. " +
            $"Body types must be chosen from the following list: 'none', 'feminine', 'masculine'. " +
            $"Accessories must be chosen from the following list: {CharacterManager.Instance.ListAccessories()}. " +
            $"Hairs must be chosen from the following list: {CharacterManager.Instance.ListHairs()}. " +
            $"Outfits must be chosen from the following list: {CharacterManager.Instance.ListOutfits()}. " +
            "Chosen outfits and accessories should be appropriate for the story's setting, if possible. For example, in a uniformed setting, all characters of the same geneder and position should be wearing the same uniform. " +
            $"Eye colors must be chosen from the following list: {CharacterManager.Instance.ListFaces()}. " +
            "Format the response as a plain JSON object with only the characters' names as top-level keys'. " +
            "Each entry under a top-level key should be an object with the keys 'BodyType', 'Hair', 'Outfit', 'Accessory', and 'Eyes'. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        //Chat.AppendSystemMessage(prompt);
        Messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        //string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(Messages);
        Messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

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
        LoadingScreen.Instance.IncrementLoadingMessage();

        string prompt =
            $"Generate a script for the next scene of a '{Genre}' genre visual novel set in the setting '{Setting}', consisting of {linesPerScene} lines of dialogue. " +
            $"Only a few characters from the cast list should appear in every scene. Some characters should be rarely appearing side characters, and the protagonist, {ProtagonistName} and Narrator should appear frequently. " +
            "The cast of the story should consist of characters from the previously generated cast list. " +
            "Each line should include the speaking character's name, the text of the dialogue, the speaker's mood, and the background image to be displayed. " +
            "Format the response as a plain JSON object with a top-level key 'DialogueLines'. " +
            "Each entry under 'DialogueLines' should be an object with the keys 'CharacterName', 'DialogueText', 'Mood', and 'BackgroundDescription'." +
            $"BackgroundsDescriptions should be chosen from the following list: {BackgroundController.Instance.ListBackgrounds()}. " +
            "Unless the story calls for a change in location, the BackgroundDescription should not change from one line of dialogue to the next. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        //Chat.AppendSystemMessage(prompt);
        Messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        DialogueScene initialDialogueScene;
        string extractedJson;

        //string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(Messages);
        Messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

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
            CharacterName = "Narrator",
            DialogueText = $"{ProtagonistName} made a choice...",
            Choice = await GenerateChoice()
        });

        return initialDialogueScene;
    }

    private async Task<Choice> GenerateChoice()
    {
        LoadingScreen.Instance.IncrementLoadingMessage();

        string prompt =
            $"From where the story left off, offer the player 3 choices of dialogue lines for the protagonist, {ProtagonistName}, to choose. " +
            "This choice should impact the trajectory of the story. " +
            "Each line should include the speaking character's name, the text of the dialogue, and the speaker's mood. " +
            "Format the response as a plain JSON object with a top-level key 'Choices'. " +
            "Each entry under 'Choices' should be an object with the keys 'CharacterName', 'DialogueText', and 'Mood'. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        //Chat.AppendSystemMessage(prompt);
        Messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        //string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(Messages);
        Messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

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
        LoadingScreen.Instance.StartLoading(LoadingState.Additional);

        string prompt = "";

        if (choiceText != null)
        {
            prompt += $"The player chose the dialogue option \"{choiceText}\". ";
        }

        prompt +=
            $"From where the story last left off, continue the visual novel's script with {linesPerScene} more lines of dialogue. ";

        //Chat.AppendSystemMessage(prompt);
        Messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        //string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(Messages);
        Messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

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
            CharacterName = "Narrator",
            DialogueText = $"{ProtagonistName} made a choice...",
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
