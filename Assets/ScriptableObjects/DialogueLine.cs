using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Custom Types/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    public string CharacterName;
    public string DialogueText;
    public string Mood;
    public Sprite BackgroundImage;
    public AudioClip VoiceLine;
    public DialogueChoice DialogueChoice;
    public string SerializedLine;
}