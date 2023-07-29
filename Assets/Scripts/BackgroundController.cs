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

        OrganizeBackgroundsByTag();
    }

    private void OrganizeBackgroundsByTag()
    {
        backgroundImage = GetComponent<Image>();
        backgroundImages.AddRange(Resources.LoadAll<BackgroundImage>(""));

        foreach (BackgroundImage bgImage in backgroundImages)
        {
            foreach (BackgroundImageTag tag in bgImage.tags)
            {
                if (!tagDictionary.ContainsKey(tag))
                {
                    tagDictionary[tag] = new List<Sprite>();
                }
                tagDictionary[tag].Add(bgImage.image);
            }
        }
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
