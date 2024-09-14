using System.Collections.Generic;
using OpenAI.Chat;

public class SaveData
{
    public string Genre;
    public string Setting;
    public string ProtagonistName;
    public Dictionary<string, CharacterDescription> CharacterDescriptions;
    public List<DialogueLine> DialoguePath;
    public int CurrentLineIndex;
    public Dictionary<string, ChatMessageRole> Messages;
}