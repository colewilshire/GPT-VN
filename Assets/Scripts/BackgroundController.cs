using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : Singleton<BackgroundController>
{
    private List<BackgroundImage> backgroundImages = new List<BackgroundImage>();
    Dictionary<BackgroundTag, List<BackgroundImage>> backgroundDictionary = new Dictionary<BackgroundTag, List<BackgroundImage>>();
    private Dictionary<string, BackgroundImage> backgroundImageCache = new Dictionary<string, BackgroundImage>();
    private Image backgroundImage;

    protected override void Awake()
    {
        base.Awake();

        OrganizeBackgroundsByTag();
    }

    private void OrganizeBackgroundsByTag()
    {
        backgroundImage = GetComponent<Image>();
        backgroundImages.AddRange(Resources.LoadAll<BackgroundImage>(""));

        TaggedImageUtility.OrganizeImagesByTag(backgroundImages, backgroundDictionary);
    }

    public BackgroundImage GetBackgroundImageWithTags(List<BackgroundTag> desiredTags)
    {
        return TaggedImageUtility.GetImageWithTags<BackgroundTag, BackgroundImage>(desiredTags, backgroundDictionary, backgroundImageCache);
    }

    public void SetBackground(Sprite sprite)
    {
        if (!sprite) return;
        backgroundImage.sprite = sprite;
    }
}
