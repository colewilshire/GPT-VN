using UnityEngine;

public class DialogueLine
{
    public string CharacterName { get; set; }
    public string DialogueText { get; set; }
    public string Mood { get; set; }
    public string BackgroundDescription { get; set; }
    public AudioClip VoiceLine { get; set; }
    public DialogueChoice DialogueChoice { get; set; }
    public Choice Choice { get; set; }
}