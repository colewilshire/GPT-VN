using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Save Data", menuName = "Custom Types/Save Data")]
public class SaveData : ScriptableObject
{
    public List<string> ConversationRoles;
    public List<string> ConversationMessages;
    public string DialoguePath;
    public List<string> CharacterNames;
    public List<string> DisplayNames;
    public List<string> CharacterAppearances;
    public List<string> BackgroundIndexes;
    public List<string> BackgroundNames;
    public int CurrentLineIndex;
}