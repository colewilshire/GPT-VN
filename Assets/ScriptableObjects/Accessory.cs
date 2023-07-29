using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Accessory", menuName = "Custom Types/Accessory")]
public class Accessory : ScriptableObject
{
    public Sprite image;
    public List<BackgroundImageTag> tags;
}