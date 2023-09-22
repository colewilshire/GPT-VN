using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Save Data", menuName = "Custom Types/Save Data")]
public class SaveData : ScriptableObject
{
    public List<string> conversationRoles;
    public List<string> conversationMessages;
    public string dialoguePath;
    public List<string> characterNames;
    public List<string> characterAppearances;
    public List<string> backgroundIndexes;
    public List<string> backgroundNames;
    public int currentLineIndex;
}