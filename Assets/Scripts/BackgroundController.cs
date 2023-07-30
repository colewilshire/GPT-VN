using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : Singleton<BackgroundController>
{
    private List<BackgroundImage> backgroundImages = new List<BackgroundImage>();
    private Dictionary<BackgroundImageTag, List<Sprite>> tagDictionary = new Dictionary<BackgroundImageTag, List<Sprite>>();
    private Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();
    private Image backgroundImage;

    protected override void Awake()
    {
        base.Awake();

        backgroundImage = GetComponent<Image>();
        backgroundImages.AddRange(Resources.LoadAll<BackgroundImage>(""));

        TagImageUtility.OrganizeImagesByTag(backgroundImages, tagDictionary);
    }

    public Sprite GetBackgroundImageWithTags(List<BackgroundImageTag> desiredTags)
    {
        return TagImageUtility.GetImageWithTags(desiredTags, tagDictionary, imageCache);
    }

    public void SetBackground(Sprite sprite)
    {
        if (!sprite) return;
        backgroundImage.sprite = sprite;
    }
}
