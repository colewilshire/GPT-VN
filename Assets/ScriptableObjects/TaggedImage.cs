using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class TaggedImage<T> : ScriptableObject where T : Enum
{
    [FormerlySerializedAs("image")]
    public Sprite MainImage;
    [FormerlySerializedAs("tags")]
    public List<T> Tags;
}