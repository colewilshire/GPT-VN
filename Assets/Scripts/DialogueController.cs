using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    //List<string> dialogue = new List<string>();
    string[] dialogue = {};

    private void Start()
    {
        ReadDialogueLine();
    }

    private async void ReadDialogueLine()
    {
        string response = await OpenAIController.Instance.Chat.GetResponseFromChatbotAsync();
        //Debug.Log(response);
        string[] lines = response.Split('\n');
        string currentLine = lines[0];
        string[] splitLine = currentLine.Split('|');

        TextController.Instance.SetText(splitLine[1]);
    }

    private void SeekThroughDialogue()
    {
        // If a dialogue exists
        // Go back one index (default index is the last index)
    }
}