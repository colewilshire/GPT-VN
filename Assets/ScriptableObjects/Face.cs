using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Face", menuName = "Custom Types/Face")]
public class Face : TaggedImage<FaceTag>
{
    [FormerlySerializedAs("image1")]
    public Sprite Sad;
    [FormerlySerializedAs("image2")]
    public Sprite Happy;
    [FormerlySerializedAs("image3")]
    public Sprite Angry;
    [FormerlySerializedAs("image4")]
    public Sprite Shocked;
    [FormerlySerializedAs("image5")]
    public Sprite Awkward;
}