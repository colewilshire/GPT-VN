using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Character Appearance", menuName = "Custom Types/Character Appearance")]
public class CharacterAppearance: ScriptableObject
{
    [FormerlySerializedAs("accessory")]
    public Accessory Accessory;
    [FormerlySerializedAs("hair")]
    public Hair Hair;
    [FormerlySerializedAs("outfit")]
    public Outfit Outfit;
    [FormerlySerializedAs("face")]
    public Face Face;
}