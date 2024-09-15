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
    private List<ChatMessage> messages;

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

        messages = new List<ChatMessage>();
        MessageDictionary = new List<KeyValuePair<ChatMessageRole, string>>();

        // Make chat requests
        LoadingScreen.Instance.StartLoading(LoadingState.Initial);
        Dictionary<string, CharacterDescription> castList = await GenerateCastList();
        DialogueScene initialDialogueScene = await GenerateInitialDialogue();

        // Overwrite protagonist appearance with custom appearance
        foreach (KeyValuePair<string, CharacterDescription> kvp in castList)
        {
            if (kvp.Key.Equals(ProtagonistName, StringComparison.OrdinalIgnoreCase))
            {
                castList[kvp.Key] = new CharacterDescription()
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

        messages = new List<ChatMessage>();
        MessageDictionary = new List<KeyValuePair<ChatMessageRole, string>>();

        foreach (KeyValuePair<ChatMessageRole, string> kvp in saveData.Messages)
        {
            if (kvp.Key == ChatMessageRole.System)
            {
                messages.Add(new SystemChatMessage(kvp.Value));
            }
            else if (kvp.Key == ChatMessageRole.Assistant)
            {
                messages.Add(new AssistantChatMessage(kvp.Value));
            }
            else
            {
                messages.Add(new UserChatMessage(kvp.Value));
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
        string accessoriesList = CharacterManager.Instance.ListAccessories();
        string hairsList = CharacterManager.Instance.ListHairs();
        string outfitsList = CharacterManager.Instance.ListOutfits();
        string facesList = CharacterManager.Instance.ListFaces();

        string prompt = PromptDefinitions.GetGenerateCastListPrompt(
            numberOfCharacters,
            Genre,
            Setting,
            ProtagonistName,
            accessoriesList,
            hairsList,
            outfitsList,
            facesList);

        messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "character_descriptions",
                jsonSchema: BinaryData.FromString(JsonSchemaDefinitions.CharacterDescriptionsSchema),
                strictSchemaEnabled: true
            )
        };

        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages, options);
        string refusal = result.Value.Content[0].Refusal;

        if (refusal != null)
        {
            Debug.Log(refusal);
            return await GenerateCastList();
        }

        messages.Add(new AssistantChatMessage(result));

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

        string backgroundsList = BackgroundController.Instance.ListBackgrounds();

        string prompt = PromptDefinitions.GetGenerateInitialDialoguePrompt(
            Genre,
            Setting,
            linesPerScene,
            ProtagonistName,
            backgroundsList);

        messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "dialogue",
                jsonSchema: BinaryData.FromString(JsonSchemaDefinitions.DialogueSchema),
                strictSchemaEnabled: true
            )
        };

        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages, options);
        string refusal = result.Value.Content[0].Refusal;

        if (refusal != null)
        {
            Debug.Log(refusal);
            return await GenerateAdditionalDialogue();
        }

        messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

        Debug.Log(assistantResponse);

        DialogueScene initialDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(assistantResponse);

        initialDialogueScene.DialogueLines.Add(new DialogueLine()
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

        string prompt = PromptDefinitions.GetGenerateChoicePrompt(ProtagonistName);

        messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "choice",
                jsonSchema: BinaryData.FromString(JsonSchemaDefinitions.ChoiceSchema),
                strictSchemaEnabled: true
            )
        };

        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages, options);
        string refusal = result.Value.Content[0].Refusal;

        if (refusal != null)
        {
            Debug.Log(refusal);
            return await GenerateChoice();
        }

        messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

        Choice choice = JsonConvert.DeserializeObject<Choice>(assistantResponse);

        Debug.Log(assistantResponse);

        return choice;
    }

    public async Task<DialogueScene> GenerateAdditionalDialogue(string choiceText = null)
    {
        LoadingScreen.Instance.StartLoading(LoadingState.Additional);

        string prompt = PromptDefinitions.GetGenerateAdditionalDialoguePrompt(choiceText, linesPerScene);

        messages.Add(new SystemChatMessage(prompt));
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.System, prompt));
        finishedPrompt += prompt;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "dialogue",
                jsonSchema: BinaryData.FromString(JsonSchemaDefinitions.DialogueSchema),
                strictSchemaEnabled: true
            )
        };

        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages, options);
        string refusal = result.Value.Content[0].Refusal;

        if (refusal != null)
        {
            Debug.Log(refusal);
            return await GenerateAdditionalDialogue();
        }

        messages.Add(new AssistantChatMessage(result));

        string assistantResponse = result.Value.Content[0].Text;
        MessageDictionary.Add(new KeyValuePair<ChatMessageRole, string>(ChatMessageRole.Assistant, assistantResponse));

        DialogueScene newDialogueScene;

        Debug.Log(assistantResponse);

        newDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(assistantResponse);
        newDialogueScene.DialogueLines.Add(new DialogueLine()
        {
            CharacterName = "Narrator",
            DialogueText = $"{ProtagonistName} made a choice...",
            Choice = await GenerateChoice()
        });

        return newDialogueScene;
    }
}
