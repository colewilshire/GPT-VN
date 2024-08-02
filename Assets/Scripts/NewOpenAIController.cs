using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        NewSaveController.Instance.ClearActiveSaveData();

        ChatRequest chatRequest = new()
        {
            Model = Model.GPT4o,
            Temperature = 1,
            MaxTokens = 4096
        };
        Chat = api.Chat.CreateConversation(chatRequest);

        // Make chat requests
        Dictionary<string, CharacterDescription> castList = await GenerateCastList();

        //string initialDialogue = await GenerateInitialDialogue();
        DialogueScene initialDialogueScene = await GenerateInitialDialogue();
        //DialogueScene initialDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(initialDialogue);

        //NewDialogueController.Instance.AddSceneToDialogue(initialDialogueScene);

        //Choice choice = await GenerateChoice();
        //Choice choices = JsonConvert.DeserializeObject<Choice>(choice);

        // Verify requests are good
        if (castList == null || initialDialogueScene == null)
        {
            StateController.Instance.SetStates(GameState.MainMenu);
            return;
        }

        // initialDialogueScene.DialogueLines.Add(new()
        // {
        //     CharacterName = "Main Character",
        //     DialogueText = "What should I choose?",
        //     Choice = choice
        // });

        // Interpret chat requests
        NewCharacterManager.Instance.GenerateCharacterPortraits(castList);
        NewDialogueController.Instance.StartDialogue(initialDialogueScene);
        NewSaveController.Instance.SaveToFile(UnityEngine.Random.Range(0, 10000).ToString());
        StateController.Instance.SetStates(GameState.Gameplay);
    }

    public async void LoadConversationFromSave(string saveName)
    {
        Dictionary<SaveDataType, List<string>> saveData = NewSaveController.Instance.LoadSaveFile(saveName);
        if (saveData == null) return;

        StateController.Instance.SetStates(GameState.Loading);
        CharacterManager.Instance.ClearCharacters();
        NewSaveController.Instance.ClearActiveSaveData();

        ChatRequest chatRequest = new()
        {
            Model = Model.GPT4o,
            Temperature = 1,
            MaxTokens = 4096
        };
        Chat = api.Chat.CreateConversation(chatRequest);

        // Make chat requests
        Dictionary<string, CharacterDescription> castList = await GenerateCastList(saveData[SaveDataType.CharacterDescriptions][0]);
        DialogueScene initialDialogueScene = await GenerateInitialDialogue(saveData);

        // Verify requests are good
        if (castList == null || initialDialogueScene == null)
        {
            StateController.Instance.SetStates(GameState.MainMenu);
            return;
        }

        // Interpret chat requests
        NewCharacterManager.Instance.GenerateCharacterPortraits(castList);
        NewDialogueController.Instance.StartDialogue(initialDialogueScene, int.Parse(saveData[SaveDataType.CurrentScene][0]));
        NewDialogueController.Instance.AddSceneToDialogue(initialDialogueScene);
        StateController.Instance.SetStates(GameState.Gameplay);
    }

    private async Task<Dictionary<string, CharacterDescription>> GenerateCastList(string saveData = null)
    {
        string prompt =
            $"Generate a cast list of {numberOfCharacters} characters for the next scene of a '{genre}' genre visual novel set in the setting '{setting}'. " +
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

        if (saveData != null)
        {
            NewSaveController.Instance.CacheData(SaveDataType.CharacterDescriptions, saveData);
            return JsonConvert.DeserializeObject<Dictionary<string, CharacterDescription>>(saveData);
        }

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

        NewSaveController.Instance.CacheData(SaveDataType.CharacterDescriptions, extractedJson);

        return characterDescriptions;
    }

    private async Task<DialogueScene> GenerateInitialDialogue(Dictionary<SaveDataType, List<string>> saveData = null)
    {
        string prompt =
            $"Generate a script for the next scene of a '{genre}' genre visual novel set in the setting '{setting}', consisting of {linesPerScene} lines of dialogue. " +
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

        if (saveData != null)
        {
            DialogueScene combinedDialogueScene = new()
            {
                DialogueLines = new()
            };

            foreach(string serializedDialogueScene in saveData[SaveDataType.DialogueScenes])
            {
                NewSaveController.Instance.CacheData(SaveDataType.DialogueScenes, serializedDialogueScene);
                DialogueScene dialogueScene = JsonConvert.DeserializeObject<DialogueScene>(serializedDialogueScene);

                Chat.AppendExampleChatbotOutput(serializedDialogueScene);
                combinedDialogueScene.DialogueLines.AddRange(dialogueScene.DialogueLines);
            }

            //NewSaveController.Instance.CacheData(SaveDataType.DialogueScenes, saveData);
            initialDialogueScene = combinedDialogueScene;
            //extractedJson = saveData;
        }
        else
        {
            string assistantResponse = await Chat.GetResponseFromChatbotAsync();
            extractedJson = ExtractJson(assistantResponse);

            Debug.Log(assistantResponse);

            try
            {
                initialDialogueScene = JsonConvert.DeserializeObject<DialogueScene>(extractedJson);
            }
            catch
            {
                // Debug.Log("Error generating usable response. Retrying.");
                // Chat.AppendSystemMessage("Formatting error. Please reread instructions and alter the format next time.");
                // return await GenerateInitialDialogue();

                Debug.Log("Error generating usable response.");
                return null;
            }

            NewSaveController.Instance.CacheData(SaveDataType.DialogueScenes, extractedJson);
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
            // Debug.Log("Error generating usable response. Retrying.");
            // Chat.AppendSystemMessage("Formatting error. Please reread instructions and alter the format next time.");
            // return await GenerateInitialDialogue();

            Debug.Log("Error generating usable response.");
            return null;
        }

        newDialogueScene.DialogueLines.Add(new()
        {
            CharacterName = "Main Character",
            DialogueText = "What should I choose?",
            Choice = await GenerateChoice()
        });

        NewSaveController.Instance.CacheData(SaveDataType.DialogueScenes, extractedJson);

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
