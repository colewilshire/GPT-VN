using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Custom Types/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    public string CharacterName;
    public string DialogueText;
    public string FacialExpression;
    public Sprite BackgroundImage;
    public AudioClip VoiceLine;
}