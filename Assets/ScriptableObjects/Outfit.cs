using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Outfit", menuName = "Custom Types/Outfit")]
public class Outfit : ScriptableObject
{
    public Sprite image;
    public List<BackgroundImageTag> tags;
}