using UnityEngine;

[CreateAssetMenu(fileName = "New Character Appearance", menuName = "Custom Types/Character Appearance")]
public class CharacterAppearance: ScriptableObject
{
    public Accessory Accessory;
    public Hair Hair;
    public Outfit Outfit;
    public Face Face;
}