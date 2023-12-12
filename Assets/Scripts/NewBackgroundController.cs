using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewBackgroundController : Singleton<NewBackgroundController>
{
    private BackgroundImage activeBackground;
    private Image displayImage;
    private Dictionary<string, BackgroundImage> backgroundImages = new();

    protected override void Awake()
    {
        base.Awake();

        displayImage = GetComponent<Image>();
        IndexbackgroundImages();
    }

    private void IndexbackgroundImages()
    {
        List<BackgroundImage> unindexedBackgroundImages = Resources.LoadAll<BackgroundImage>("BackgroundImages").ToList();
        backgroundImages = new();

        foreach(BackgroundImage backgroundImage in unindexedBackgroundImages)
        {
            backgroundImages[backgroundImage.Description] = backgroundImage;
        }
    }

    public void ShowBackground(string backgroundDescription)
    {
        backgroundDescription = backgroundDescription.ToLower().Trim('\'').Trim();

        if (backgroundImages.TryGetValue(backgroundDescription, out activeBackground))
        {
            displayImage.sprite = activeBackground.MainImage;
        }
        else
        {
            activeBackground = null;
            displayImage.sprite = null;
        }
    }

    public string ListBackgrounds()
    {
        string listedBackgrounds = "";

        foreach(KeyValuePair<string, BackgroundImage> keyValuePair in backgroundImages)
        {
            string backgroundDescription = $"'{keyValuePair.Key}'";
            listedBackgrounds += $"{backgroundDescription}, ";
        }

        listedBackgrounds = listedBackgrounds.TrimEnd(',', ' ');

        return listedBackgrounds;
    }
}
