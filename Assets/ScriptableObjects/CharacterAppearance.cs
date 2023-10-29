using UnityEngine;

[CreateAssetMenu(fileName = "New Character Appearance", menuName = "Custom Types/Character Appearance")]
public class CharacterAppearance: ScriptableObject
{
    public Accessory Accessory = new();
    public Hair Hair = new();
    public Outfit Outfit = new();
    public Face Face = new();
}