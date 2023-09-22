using UnityEngine;

[CreateAssetMenu(fileName = "New Hair", menuName = "Custom Types/Hair")]
public class Hair : TaggedImage<HairTag>
{
    public Sprite HairBackground;
}