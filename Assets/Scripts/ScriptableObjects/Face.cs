using UnityEngine;

[CreateAssetMenu(fileName = "New Face", menuName = "Custom Types/Face")]
public class Face : TaggedImage<FaceTag>
{
    public Sprite Sad;
    public Sprite Happy;
    public Sprite Angry;
    public Sprite Shocked;
    public Sprite Awkward;
}