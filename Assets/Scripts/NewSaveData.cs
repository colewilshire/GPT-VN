using System.Collections.Generic;
using OpenAI_API.Chat;

public class NewSaveData
{
    public Dictionary<string, CharacterDescription> CharacterDescriptions;
    public List<NewDialogueLine> DialoguePath;
    public int CurrentLineIndex;
    public IList<ChatMessage> Messages;
}