using UnityEngine;

[CreateAssetMenu(fileName = "New Character Appearance", menuName = "Custom Types/Character Appearance")]
public class CharacterAppearance: ScriptableObject
{
    public Sprite accessory;
    public Sprite hairFront;
    public Sprite outfit;
    public Sprite face;
    public Sprite hairBack;
}