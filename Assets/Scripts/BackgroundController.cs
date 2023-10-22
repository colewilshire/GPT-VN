using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : Singleton<BackgroundController>
{
    private List<BackgroundImage> backgroundImages = new();
    private Dictionary<BackgroundTag, List<BackgroundImage>> backgroundDictionary = new();
    private Dictionary<string, BackgroundImage> backgroundImageCache = new();
    private Image backgroundImage;
    public Dictionary<string, BackgroundImage> GeneratedBackgrounds {get; private set;} = new Dictionary<string, BackgroundImage>();

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

    public void GenerateBackgroundImages(Dictionary<string, List<BackgroundTag>> tagsByBackground)
    {
        GeneratedBackgrounds = new Dictionary<string, BackgroundImage>();

        foreach (KeyValuePair<string, List<BackgroundTag>> kvp in tagsByBackground)
        {
            GeneratedBackgrounds[kvp.Key] = GetBackgroundImageWithTags(kvp.Value);
        }
    }

    public Sprite GetBackgroundImageFromName(string backgroundName)
    {
        if (GeneratedBackgrounds.TryGetValue(backgroundName, out BackgroundImage foundImage))
        {
            if (foundImage == null) return null;
            return foundImage.MainImage;
        }

        return null;
    }

    public void LoadBackgroundImagesFromSave(SaveData saveData)
    {
        GeneratedBackgrounds = new Dictionary<string, BackgroundImage>();

        for (int i = 0; i < saveData.BackgroundIndexes.Count; ++i)
        {
            string backgroundIndex = saveData.BackgroundIndexes[i];
            string backgroundName = saveData.BackgroundNames[i];
            BackgroundImage foundImage = Resources.Load<BackgroundImage>($"BackgroundImages/{backgroundName}");

            GeneratedBackgrounds[backgroundIndex] = foundImage;
        }
    }
}
