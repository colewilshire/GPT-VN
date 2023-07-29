using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Face", menuName = "Custom Types/Face")]
public class Face : ScriptableObject
{
    public Sprite image;
    public List<BackgroundImageTag> tags;
}