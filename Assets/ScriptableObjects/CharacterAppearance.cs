using UnityEngine;

[CreateAssetMenu(fileName = "New Character Appearance", menuName = "Custom Types/Character Appearance")]
public class CharacterAppearance: ScriptableObject
{
    public Accessory accessory;
    public Hair hair;
    public Outfit outfit;
    public Face face;
}