using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : Singleton<BackgroundController>
{
    private List<BackgroundImage> backgroundImages = new List<BackgroundImage>();
    private Dictionary<BackgroundTag, List<Sprite>> backgroundDictionary = new Dictionary<BackgroundTag, List<Sprite>>();
    private Dictionary<string, Sprite> backgroundImageCache = new Dictionary<string, Sprite>();
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

    public Sprite GetBackgroundImageWithTags(List<BackgroundTag> desiredTags)
    {
        return TaggedImageUtility.GetImageWithTags(desiredTags, backgroundDictionary, backgroundImageCache);
    }

    public void SetBackground(Sprite sprite)
    {
        if (!sprite) return;
        backgroundImage.sprite = sprite;
    }
}
