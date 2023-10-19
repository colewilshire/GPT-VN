using OpenAI_API;
using OpenAI_API.Chat;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenAIController : Singleton<OpenAIController>
{
    [SerializeField] private string apiKey;
    [SerializeField] private string genre = "romance";
    [SerializeField] private string setting = "high school";
    [SerializeField] private int linesPerScene = 10;
    [SerializeField] private int numberOfCharacters = 10;
    [SerializeField] private string finishedPrompt = "";
    private OpenAIAPI api;
    public Conversation Chat { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        api = new OpenAIAPI(apiKey);
    }

    private void PromptWithInitialInstructions()
    {
        string prompt =
            $"You are generating content for a {genre}-themed visual novel, set in {setting}. " +
            //"The visual novel's script is split into an indefinite number of short acts, which heavily narratively connect to each other. " +
            "The story will largely follow a small group of core characters, with occassional side characters interspersed as needed. " +
            $"There must be no more than {numberOfCharacters}, including the Narrator.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;
    }

    private async Task<string> GenerateCastList()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.Cast);

        string prompt =
            "Generate a cast list of characters for the visual novel. " +
            "The cast list must contain only the characters' names, outputted in a single line in Pipe-Separated Vale (PSV) format.";
            //"It is mandatory that the cast list include a character named \"Narrator\" who serves as the game's narrator.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateHairDescriptions()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.Hair);

        Type type = typeof(HairTag);;
        string prompt =
            "For each character in the cast list, describe their hair using terms only from the following list: " +
            GetEnumValues(type) +
            "Each character's hair description should be outputted in PSV format, starting with the character's name, followed by any number of applicable hair traits from the list, each separated by a '|'.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateOutfitDescriptions()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.Outfits);

        Type type = typeof(OutfitTag);
        string prompt =
            "For each character in the cast list, describe their outfit using terms only from the following list: " +
            GetEnumValues(type) +
            "Each character's outfit description should be outputted in PSV format, starting with the character's name, followed by any number of applicable outfit traits from the list, each separated by a '|'.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateEyeColorDescriptions()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.EyeColors);
        
        Type type = typeof(FaceTag);
        string prompt =
            "For each character in the cast list, describe their eye color using only one term from the following list: " +
            GetEnumValues(type) +
            "Each character's eye color description should be outputted in PSV format, starting with the character's name, followed by one eye color from the list, separated by a '|'.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateAccessoryDescriptions()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.Accessories);

        Type type = typeof(AccessoryTag);
        string prompt =
            "For each character in the cast list, describe their accessories using terms only from the following list: " +
            GetEnumValues(type) +
            "Each character's accessories description should be outputted in PSV format, starting with the character's name, followed by any number of applicable accessories from the list, each separated by a '|'.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateDialogue()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.Dialogue);

        Type mood = typeof(Mood);
        string prompt =
            "Create a script for the next act of the visual novel. " +
            $"The script should be {linesPerScene} lines of dialogue long. " +
            "Dialogue lines should start with the speaking character's name, followed by the dialogue's text, followed by the speaker's emotion, followed by the name of the background to display, each separated by a '|'." +
            "Each line of dialogue should include a single emotion. The chosen emotion must come from the following list: " +
            GetEnumValues(mood) +
            " Backgrounds names should come from the previously generated list of backgrounds.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> ContinueDialogue()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.AdditionalDialogue);

        string prompt =
            $"From where the story last left off, continue the visual novel's script with {linesPerScene} more lines of dialogue. ";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    public async Task<string> GenerateChoice()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.AdditionalDialogue);

        string prompt =
            "From where the story last left off, offer the player 3 choices of dialogue lines they may choose for the main character to speak to determine the trajectory of the story. " +
            "Dialogue lines should start with the speaking character's name, followed by the dialogue's text, followed by the speaker's emotion, followed by the name of the background to display, each separated by a '|'.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        assistantResponse = $"©{assistantResponse.Trim('\n').Replace('\n', '©')}";

        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateBackgroundDescription()
    {
        LoadingScreen.Instance.SetLoadingState(LoadingState.Backgrounds);

        Type type = typeof(BackgroundTag);
        string prompt =
            "Generate a list of names for background images that could be featured in the visual novel. " +
            "Describe these backgrounds using any number terms, exclusively from the following list: " +
            GetEnumValues(type) +
            "The background image description should be outputted in PSV format, starting with its name, followed by any number of applicable background image traits from the list, each separated by a '|'.";
        Chat.AppendSystemMessage(prompt);
        finishedPrompt += prompt;

        string assistantResponse = await Chat.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private string GetEnumValues(Type enumType)
    {
        string enumValues = "";
        Array values = Enum.GetValues(enumType);
        List<string> valueList = new List<string>();

        foreach (var value in values)
        {
            valueList.Add(value.ToString());
        }

        enumValues += string.Join(", ", valueList) + ". ";

        return enumValues;
    }

    private static Dictionary<string, List<TEnum>> DeserializeTags<TEnum>(string serializedTags) where TEnum : struct, Enum
    {
        Dictionary<string, List<TEnum>> tagsByCharacter = new Dictionary<string, List<TEnum>>();
        List<string> splitSerializedTags = new List<string>(serializedTags.Split('\n', StringSplitOptions.RemoveEmptyEntries));

        foreach (string characterSerializedTags in splitSerializedTags)
        {
            List<string> tags = new List<string>(characterSerializedTags.Split('|', StringSplitOptions.RemoveEmptyEntries))
                .Select(str => str.Trim())
                .ToList();
            string characterName = tags[0];

            tags.RemoveAt(0);

            List<TEnum> enumTags = new List<TEnum>();
            foreach (string tag in tags)
            {
                if (Enum.TryParse(tag, out TEnum parsedTag))
                {
                    enumTags.Add(parsedTag);
                }
            }

            tagsByCharacter[characterName] = enumTags;
        }

        return tagsByCharacter;
    }

    public async void CreateNewConversation()
    {
        StateController.Instance.SetState(GameState.Loading);
        CharacterManager.Instance.ClearCharacters();

        ChatRequest chatRequest = new ChatRequest();
        //chatRequest.Model = Model.GPT4;
        Chat = api.Chat.CreateConversation(chatRequest);

        PromptWithInitialInstructions();
        string serializedCastList = await GenerateCastList();
        string serializedDialogue = await GenerateDialogue();
        string serializedChoice = await GenerateChoice();
        string serializedHairDescriptions = await GenerateHairDescriptions();
        string serializedOutfitDescriptions = await GenerateOutfitDescriptions();
        string serializedEyeColorDescriptions = await GenerateEyeColorDescriptions();
        string serializedAccessoryDescriptions = await GenerateAccessoryDescriptions();
        string serializedBackgroundDescriptions = await GenerateBackgroundDescription();

        List<string> castList = serializedCastList.Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(str => str.Trim())
            .ToList();
        Dictionary<string, List<HairTag>> hairDescriptions = DeserializeTags<HairTag>(serializedHairDescriptions);
        Dictionary<string, List<OutfitTag>> outfitDescriptions = DeserializeTags<OutfitTag>(serializedOutfitDescriptions);
        Dictionary<string, List<FaceTag>> eyeColorDescriptions = DeserializeTags<FaceTag>(serializedEyeColorDescriptions);
        Dictionary<string, List<AccessoryTag>> accessoryDescriptions = DeserializeTags<AccessoryTag>(serializedAccessoryDescriptions);
        Dictionary<string, List<BackgroundTag>> backgroundDescriptions = DeserializeTags<BackgroundTag>(serializedBackgroundDescriptions);

        foreach (string characterName in castList)
        {
            List<HairTag> hairTags;
            List<OutfitTag> outfitTags;
            List<FaceTag> eyeColorTags;
            List<AccessoryTag> accessoryTags;

            hairDescriptions.TryGetValue(characterName, out hairTags);
            outfitDescriptions.TryGetValue(characterName, out outfitTags);
            eyeColorDescriptions.TryGetValue(characterName, out eyeColorTags);
            accessoryDescriptions.TryGetValue(characterName, out accessoryTags);

            if (hairTags != null && outfitTags != null && eyeColorTags != null && accessoryTags != null)
            {
                CharacterGenerationController.Instance.GenerateCharacterPortrait(characterName, accessoryTags, hairTags, outfitTags, eyeColorTags);
            }
            else
            {
                Debug.Log($"Error generating {characterName}.");
            }
        }

        BackgroundController.Instance.GenerateBackgroundImages(backgroundDescriptions);
        DialogueController.Instance.StartDialogue(serializedDialogue);
        DialogueController.Instance.AddToDialogue(serializedChoice);
        StateController.Instance.SetState(GameState.Gameplay);
    }

    public void LoadConversationFromSave(string saveName)
    {
        StateController.Instance.SetState(GameState.Loading);
        CharacterManager.Instance.ClearCharacters();

        SaveData saveData = SaveController.Instance.Load(saveName);

        if (!saveData)
        {
            StateController.Instance.SetState(GameState.MainMenu);
            return;
        }

        ChatRequest chatRequest = new ChatRequest();
        //chatRequest.Model = Model.GPT4;
        Chat = api.Chat.CreateConversation(chatRequest);

        for (int i = 0; i < saveData.ConversationRoles.Count; ++i)
        {
            string roleName = saveData.ConversationRoles[i];
            string message = saveData.ConversationMessages[i];
            ChatMessageRole role = ChatMessageRole.FromString(roleName);
            ChatMessage chatMessage = new ChatMessage(role, message);

            Chat.AppendMessage(chatMessage);
        }

        CharacterGenerationController.Instance.LoadCharactersFromSave(saveData);
        BackgroundController.Instance.LoadBackgroundImagesFromSave(saveData);
        DialogueController.Instance.LoadDialogueFromSave(saveData);
        StateController.Instance.SetState(GameState.Gameplay);
    }

    public async void GenerateAdditionalDialogue()
    {
        StateController.Instance.SetState(GameState.Loading);

        string additionalDialogue = await ContinueDialogue();
        string additionalChoice = await GenerateChoice();
        DialogueController.Instance.ContinueDialogue(additionalDialogue);
        DialogueController.Instance.AddToDialogue(additionalChoice);

        StateController.Instance.SetState(GameState.Gameplay);
    }
}
