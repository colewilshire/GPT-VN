using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewBackgroundController : Singleton<NewBackgroundController>
{
    private BackgroundImage activeBackground;
    private Image displayImage;
    private readonly Dictionary<string, BackgroundImage> backgroundImages = new();

    protected override void Awake()
    {
        base.Awake();

        displayImage = GetComponent<Image>();
        SortBackgroundImages();
    }

    private void SortBackgroundImages()
    {
        List<BackgroundImage> unsortedBackgroundImages = Resources.LoadAll<BackgroundImage>("BackgroundImages").ToList();

        foreach(BackgroundImage backgroundImage in unsortedBackgroundImages)
        {
            backgroundImages[backgroundImage.Description] = backgroundImage;
        }
    }

    public void ShowBackground(string backgroundDescription)
    {
        if (backgroundDescription == null) return;

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
