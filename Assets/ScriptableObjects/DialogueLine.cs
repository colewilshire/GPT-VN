using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Custom Types/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    [FormerlySerializedAs("characterName")]
    public string CharacterName;
    [FormerlySerializedAs("dialogueText")]
    public string DialogueText;
    [FormerlySerializedAs("mood")]
    public string FacialExpression;
    [FormerlySerializedAs("backgroundImage")]
    public Sprite BackgroundImage;
    [FormerlySerializedAs("voiceLine")]
    public AudioClip VoiceLine;
}