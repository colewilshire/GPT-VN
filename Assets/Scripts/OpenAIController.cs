using System;
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
    [SerializeField] private string model;  // On 10/2, the default version of GPT-4o will be updated to gpt-4o-2024-08-06. I will need to update the model name to "gpt-4o"

    [Header("Story Settings")]
    [SerializeField] private int linesPerScene = 10;
    [SerializeField] private int numberOfCharacters = 5;

    [Header("Debug")]
    [SerializeField] private string finishedPrompt;

    private ChatClient client;
    private List<ChatMessage> Messages;

    public string Genre { get; private set; }
    public string Setting { get; private set; }
    public string ProtagonistName { get; private set; }
    public List<KeyValuePair<ChatMessageRole, string>> MessageDictionary;

    protected override void Awake()
    {
        base.Awake();

        client = new ChatClient(model, apiKey);
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

        // Overwrite protagonist appearance with custom appearance
        foreach (KeyValuePair<string, CharacterDescription> kvp in castList)
        {
            if (kvp.Key.ToLower() == ProtagonistName.ToLower())
            {
                castList[kvp.Key] = new()
                {
                    Name = ProtagonistName,
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
            $"Eye colors must be chosen from the following list: {CharacterManager.Instance.ListFaces()}. ";

        Messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "character_descriptions",
                jsonSchema: BinaryData.FromString(
                    "{\n" +
                    "  \"type\": \"object\",\n" +
                    "  \"properties\": {\n" +
                    "    \"CharacterDescriptions\": {\n" +
                    "      \"type\": \"array\",\n" +
                    "      \"items\": {\n" +
                    "        \"type\": \"object\",\n" +
                    "        \"properties\": {\n" +
                    "          \"Name\": { \"type\": \"string\" },\n" +
                    "          \"BodyType\": { \"type\": \"string\" },\n" +
                    "          \"Hair\": { \"type\": \"string\" },\n" +
                    "          \"Outfit\": { \"type\": \"string\" },\n" +
                    "          \"Accessory\": { \"type\": \"string\" },\n" +
                    "          \"Eyes\": { \"type\": \"string\" }\n" +
                    "        },\n" +
                    "        \"required\": [\"Name\", \"BodyType\", \"Hair\", \"Outfit\", \"Accessory\", \"Eyes\"],\n" +
                    "        \"additionalProperties\": false\n" +
                    "      }\n" +
                    "    }\n" +
                    "  },\n" +
                    "  \"required\": [\"CharacterDescriptions\"],\n" +
                    "  \"additionalProperties\": false\n" +
                    "}"
                ),
                strictSchemaEnabled: true
            )
        };
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(Messages, options);
        string refusal = result.Value.Content[0].Refusal;

        if (refusal != null)
        {
            Debug.Log(refusal);
            return await GenerateCastList();
        }

        Messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

        Debug.Log(assistantResponse);

        Dictionary<string, List<CharacterDescription>> characterDescriptions = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<CharacterDescription>>>(assistantResponse);
        Dictionary<string, CharacterDescription> formattedCharacterDescriptions = new();

        foreach (CharacterDescription characterDescription in characterDescriptions["CharacterDescriptions"])
        {
            formattedCharacterDescriptions[characterDescription.Name] = characterDescription;
        }

        return formattedCharacterDescriptions;
    }

    private async Task<DialogueScene> GenerateInitialDialogue()
    {
        LoadingScreen.Instance.IncrementLoadingMessage();

        string prompt =
            $"Generate a script for the next scene of a '{Genre}' genre visual novel set in the setting '{Setting}', consisting of {linesPerScene} lines of dialogue. " +
            $"Only a few characters from the cast list should appear in every scene. Some characters should be rarely appearing side characters, and the protagonist, {ProtagonistName} and Narrator should appear frequently. " +
            "The cast of the story should consist of characters from the previously generated cast list. " +
            "Each line should include the speaking character's name, the text of the dialogue, the speaker's mood, and the background image to be displayed. " +
            "Format the response as a plain JSON object with a top-level key 'Dialogue'. " +
            "Each entry under 'Dialogue' should be an object with the keys 'CharacterName', 'DialogueText', 'Mood', and 'BackgroundDescription'." +
            $"BackgroundsDescriptions should be chosen from the following list: {BackgroundController.Instance.ListBackgrounds()}. " +
            "Unless the story calls for a change in location, the BackgroundDescription should not change from one line of dialogue to the next. " +
            "Do not include any additional formatting or markers such as markdown code block markers.";

        Messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "dialogue",
                jsonSchema: BinaryData.FromString(
                    "{\n" +
                    "  \"type\": \"object\",\n" +
                    "  \"properties\": {\n" +
                    "    \"DialogueLines\": {\n" +
                    "      \"type\": \"array\",\n" +
                    "      \"items\": {\n" +
                    "        \"type\": \"object\",\n" +
                    "        \"properties\": {\n" +
                    "          \"CharacterName\": { \"type\": \"string\" },\n" +
                    "          \"DialogueText\": { \"type\": \"string\" },\n" +
                    "          \"Mood\": { \"type\": \"string\" },\n" +
                    "          \"BackgroundDescription\": { \"type\": \"string\" }\n" +
                    "        },\n" +
                    "        \"required\": [\"CharacterName\", \"DialogueText\", \"Mood\", \"BackgroundDescription\"],\n" +
                    "        \"additionalProperties\": false\n" +
                    "      }\n" +
                    "    }\n" +
                    "  },\n" +
                    "  \"required\": [\"DialogueLines\"],\n" +
                    "  \"additionalProperties\": false\n" +
                    "}"
                ),
                strictSchemaEnabled: true
            )
        };
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(Messages, options);
        string refusal = result.Value.Content[0].Refusal;

        if (refusal != null)
        {
            Debug.Log(refusal);
            return await GenerateAdditionalDialogue();
        }

        Messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

        Debug.Log(assistantResponse);

        DialogueScene initialDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(assistantResponse);

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

        Messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "choice",
                jsonSchema: BinaryData.FromString(
                    "{\n" +
                    "  \"type\": \"object\",\n" +
                    "  \"properties\": {\n" +
                    "    \"Choices\": {\n" +
                    "      \"type\": \"array\",\n" +
                    "      \"items\": {\n" +
                    "        \"type\": \"object\",\n" +
                    "        \"properties\": {\n" +
                    "          \"CharacterName\": { \"type\": \"string\" },\n" +
                    "          \"DialogueText\": { \"type\": \"string\" },\n" +
                    "          \"Mood\": { \"type\": \"string\" },\n" +
                    "          \"BackgroundDescription\": { \"type\": \"string\" }\n" +
                    "        },\n" +
                    "        \"required\": [\"CharacterName\", \"DialogueText\", \"Mood\", \"BackgroundDescription\"],\n" +
                    "        \"additionalProperties\": false\n" +
                    "      }\n" +
                    "    }\n" +
                    "  },\n" +
                    "  \"required\": [\"Choices\"],\n" +
                    "  \"additionalProperties\": false\n" +
                    "}"
                ),
                strictSchemaEnabled: true
            )
        };
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(Messages);
        string refusal = result.Value.Content[0].Refusal;

        if (refusal != null)
        {
            Debug.Log(refusal);
            return await GenerateChoice();
        }

        Messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

        Choice choice = JsonConvert.DeserializeObject<Choice>(assistantResponse);

        Debug.Log(assistantResponse);

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

        Messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "dialogue",
                jsonSchema: BinaryData.FromString(
                    "{\n" +
                    "  \"type\": \"object\",\n" +
                    "  \"properties\": {\n" +
                    "    \"DialogueLines\": {\n" +
                    "      \"type\": \"array\",\n" +
                    "      \"items\": {\n" +
                    "        \"type\": \"object\",\n" +
                    "        \"properties\": {\n" +
                    "          \"CharacterName\": { \"type\": \"string\" },\n" +
                    "          \"DialogueText\": { \"type\": \"string\" },\n" +
                    "          \"Mood\": { \"type\": \"string\" },\n" +
                    "          \"BackgroundDescription\": { \"type\": \"string\" }\n" +
                    "        },\n" +
                    "        \"required\": [\"CharacterName\", \"DialogueText\", \"Mood\", \"BackgroundDescription\"],\n" +
                    "        \"additionalProperties\": false\n" +
                    "      }\n" +
                    "    }\n" +
                    "  },\n" +
                    "  \"required\": [\"DialogueLines\"],\n" +
                    "  \"additionalProperties\": false\n" +
                    "}"
                ),
                strictSchemaEnabled: true
            )
        };
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(Messages, options);
        string refusal = result.Value.Content[0].Refusal;

        if (refusal != null)
        {
            Debug.Log(refusal);
            return await GenerateAdditionalDialogue();
        }

        Messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

        DialogueScene newDialogueScene;

        Debug.Log(assistantResponse);

        newDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(assistantResponse);
        newDialogueScene.DialogueLines.Add(new()
        {
            CharacterName = "Narrator",
            DialogueText = $"{ProtagonistName} made a choice...",
            Choice = await GenerateChoice()
        });

        return newDialogueScene;
    }
}
