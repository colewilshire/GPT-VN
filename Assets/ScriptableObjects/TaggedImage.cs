using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaggedImage<T> : ScriptableObject where T : Enum
{
    public Sprite MainImage;
    public List<T> Tags;
}