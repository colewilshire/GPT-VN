using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Face", menuName = "Custom Types/Face")]
public class Face : TaggedImage<FaceTag>
{
    [FormerlySerializedAs("MainImage1")]
    public Sprite MainImage1;
    [FormerlySerializedAs("MainImage2")]
    public Sprite MainImage2;
    [FormerlySerializedAs("MainImage3")]
    public Sprite MainImage3;
    [FormerlySerializedAs("MainImage4")]
    public Sprite MainImage4;
    [FormerlySerializedAs("MainImage5")]
    public Sprite MainImage5;
}