using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Choice", menuName = "Custom Types/Dialogue Choice")]
public class DialogueChoice : ScriptableObject
{
    public List<DialogueLine> Choices = new();
}