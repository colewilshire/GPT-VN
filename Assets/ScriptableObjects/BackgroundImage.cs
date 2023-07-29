using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Background Image", menuName = "Custom Types/Background Image")]
public class BackgroundImage : ScriptableObject
{
    public Sprite image;
    public List<BackgroundImageTag> tags;
}