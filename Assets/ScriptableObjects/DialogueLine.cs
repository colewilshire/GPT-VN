using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Custom Types/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    [FormerlySerializedAs("characterName")]
    public string characterName;
    [FormerlySerializedAs("dialogueText")]
    public string dialogueText;
    [FormerlySerializedAs("mood")]
    public string mood;
    [FormerlySerializedAs("backgroundImage")]
    public Sprite backgroundImage;
    [FormerlySerializedAs("voiceLine")]
    public AudioClip voiceLine;
}