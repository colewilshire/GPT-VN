using System;
using UnityEngine;

[Serializable]
public class NewDialogueLine
{
    public string CharacterName;
    public string DialogueText;
    public string Mood;
    public Sprite BackgroundImage;
    public AudioClip VoiceLine;
    public DialogueChoice DialogueChoice;
    public string SerializedLine;
}