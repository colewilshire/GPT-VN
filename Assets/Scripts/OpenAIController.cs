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
    private OpenAIAPI api;
    private Conversation conversation;

    protected override void Awake()
    {
        base.Awake();

        api = new OpenAIAPI(apiKey);
    }

    private void Start()
    {
        CreateConversation();
    }

    private async void CreateConversation()
    {
        conversation = api.Chat.CreateConversation();

        PromptWithInitialInstructions();
        string serializedCastList = await GenerateCastList();
        string serializedHairDescriptions = await GenerateHairDescriptions();
        string serializedOutfitDescriptions = await GenerateOutfitDescriptions();
        string serializedEyeColorDescriptions = await GenerateEyeColorDescriptions();
        string serializedAccessoryDescriptions = await GenerateAccessoryDescriptions();
        string serializedDialogue = await GenerateDialogue();
        string serializedBackgroundDescription = await GenerateBackgroundDescription();
        string serializedDialogue2 = await ContinueDialogue();
        string serializedBackgroundDescription2 = await GenerateBackgroundDescription();

        List<string> castList = serializedCastList.Split('|').Select(str => str.Trim()).ToList();
        Dictionary<string, List<HairTag>> hairDescriptions = DeserializeTags<HairTag>(serializedHairDescriptions);
        Dictionary<string, List<OutfitTag>> outfitDescriptions = DeserializeTags<OutfitTag>(serializedOutfitDescriptions);
        Dictionary<string, List<FaceTag>> eyeColorDescriptions = DeserializeTags<FaceTag>(serializedEyeColorDescriptions);
        Dictionary<string, List<AccessoryTag>> accessoryDescriptions = DeserializeTags<AccessoryTag>(serializedAccessoryDescriptions);

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
                Debug.Log($"Error generating {characterName}");
            }
        }
    }

    private void PromptWithInitialInstructions()
    {
        string prompt =
            $"You are generating content for a {genre}-themed visual novel, set in {setting}. " +
            //"The visual novel's script is split into an indefinite number of short acts, which heavily narratively connect to each other. " +
            "The story will largely follow a small group of core characters, with occassional side characters interspersed as needed. " +
            $"There must be no more than {numberOfCharacters}, including the Narrator.";
        conversation.AppendSystemMessage(prompt);
    }

    private async Task<string> GenerateCastList()
    {
        string prompt =
            "Generate a cast list of characters for the visual novel. " +
            "The cast list must contain only the characters' names, outputted in a single line in Pipe-Separated Vale (PSV) format.";
            //"It is mandatory that the cast list include a character named \"Narrator\" who serves as the game's narrator.";
        conversation.AppendSystemMessage(prompt);

        string assistantResponse = await conversation.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateHairDescriptions()
    {
        Type type = typeof(HairTag);;
        string prompt =
            "For each character in the cast list, describe their hair using terms only from the following list: " +
            GetEnumValues(type) +
            "Each character's hair description should be outputted in PSV format, starting with the character's name, followed by any number of applicable hair traits from the list, each separated by a '|'.";
        conversation.AppendSystemMessage(prompt);

        string assistantResponse = await conversation.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateOutfitDescriptions()
    {
        Type type = typeof(OutfitTag);
        string prompt =
            "For each character in the cast list, describe their outfit using terms only from the following list: " +
            GetEnumValues(type) +
            "Each character's outfit description should be outputted in PSV format, starting with the character's name, followed by any number of applicable outfit traits from the list, each separated by a '|'.";
        conversation.AppendSystemMessage(prompt);

        string assistantResponse = await conversation.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateEyeColorDescriptions()
    {
        
        Type type = typeof(FaceTag);
        string prompt =
            "For each character in the cast list, describe their eye color using only one term from the following list: " +
            GetEnumValues(type) +
            "Each character's eye color description should be outputted in PSV format, starting with the character's name, followed by one eye color from the list, separated by a '|'.";
        conversation.AppendSystemMessage(prompt);

        string assistantResponse = await conversation.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateAccessoryDescriptions()
    {
        Type type = typeof(AccessoryTag);
        string prompt =
            "For each character in the cast list, describe their accessories using terms only from the following list: " +
            GetEnumValues(type) +
            "Each character's accessories description should be outputted in PSV format, starting with the character's name, followed by any number of applicable accessories from the list, each separated by a '|'.";
        conversation.AppendSystemMessage(prompt);

        string assistantResponse = await conversation.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateDialogue()
    {
        string prompt =
            "Create a script for the next act of the visual novel. " +
            $"The script should be {linesPerScene} lines of dialogue long. " +
            "Dialogue lines should start with the speaking character's name, followed by the dialogue's text, separated by a '|'.";
        conversation.AppendSystemMessage(prompt);

        string assistantResponse = await conversation.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> ContinueDialogue()
    {
        string prompt =
            $"From where the story last left off, continue the visual novel's script with {linesPerScene} more lines of dialogue. ";
        conversation.AppendSystemMessage(prompt);

        string assistantResponse = await conversation.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private async Task<string> GenerateBackgroundDescription()
    {
        Type type = typeof(BackgroundTag);
        string prompt =
            "Describe the background image for the last scene using only terms from the following list: " +
            GetEnumValues(type) +
            "The background image description should be outputted in PSV format, including any number of applicable background image traits from the list, each separated by a '|'.";
        conversation.AppendSystemMessage(prompt);

        string assistantResponse = await conversation.GetResponseFromChatbotAsync();
        Debug.Log(assistantResponse);

        return assistantResponse;
    }

    private string GetEnumValues(Type enumType)
    {
        //string enumName = enumType.Name;
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
        List<string> splitSerializedTags = new List<string>(serializedTags.Split('\n'));

        foreach (string characterSerializedTags in splitSerializedTags)
        {
            List<string> tags = new List<string>(characterSerializedTags.Split('|')).Select(str => str.Trim()).ToList();;
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
}