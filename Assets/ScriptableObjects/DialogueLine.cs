using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Custom Types/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    public string characterName;
    public string dialogueText;
    public string mood;
    public string backgroundName;
    public AudioClip voiceLine;
    public Sprite backgroundImage;
}