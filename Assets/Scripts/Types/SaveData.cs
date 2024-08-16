using System.Collections.Generic;
using OpenAI_API.Chat;

public class SaveData
{
    public string Genre;
    public string Setting;
    public Dictionary<string, CharacterDescription> CharacterDescriptions;
    public List<DialogueLine> DialoguePath;
    public int CurrentLineIndex;
    public IList<ChatMessage> Messages;
}