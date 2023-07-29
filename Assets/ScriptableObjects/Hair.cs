using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hair", menuName = "Custom Types/Hair")]
public class Hair : ScriptableObject
{
    public Sprite hairFront;
    public Sprite hairBack;
    public List<BackgroundImageTag> tags;
}