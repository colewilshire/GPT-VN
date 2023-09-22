using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Hair", menuName = "Custom Types/Hair")]
public class Hair : TaggedImage<HairTag>
{
    [FormerlySerializedAs("imageBackground")]
    public Sprite HairBackground;
}