using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitController : MonoBehaviour
{
    [SerializeField] private Image accessory;
    [SerializeField] private Image hairFront;
    [SerializeField] private Image outfit;
    [SerializeField] private Image face;
    [SerializeField] private Image hairBack;
    public CharacterAppearance Appearance { get; private set; }

    private void SetExpression(Mood expression)
    {
        if(!face.sprite) return;

        switch (expression)
        {
            case Mood.Neutral:
                face.sprite = Appearance.Face.MainImage;
                break;
            case Mood.Sad:
                face.sprite = Appearance.Face.MainImage1 ?? Appearance.Face.MainImage;
                break;
            case Mood.Happy:
                face.sprite = Appearance.Face.MainImage2 ?? Appearance.Face.MainImage;
                break;
            case Mood.Angry:
                face.sprite = Appearance.Face.MainImage3 ?? Appearance.Face.MainImage;
                break;
            case Mood.Shocked:
                face.sprite = Appearance.Face.MainImage4 ?? Appearance.Face.MainImage;
                break;
            case Mood.Awkward:
                face.sprite = Appearance.Face.MainImage5 ?? Appearance.Face.MainImage;
                break;
            default:
                face.sprite = Appearance.Face.MainImage;
                break;
        }
    }

    public void SetAppearance(CharacterAppearance characterAppearance)
    {
        Appearance = characterAppearance;

        accessory.sprite = characterAppearance.Accessory?.MainImage ?? null;
        hairFront.sprite = characterAppearance.Hair?.MainImage ?? null;
        outfit.sprite = characterAppearance.Outfit?.MainImage ?? null;
        face.sprite = characterAppearance.Face?.MainImage ?? null;
        hairBack.sprite = characterAppearance.Hair?.MainImageBackground ?? null;

        accessory.gameObject.SetActive(accessory.sprite);
        hairFront.gameObject.SetActive(hairFront.sprite);
        outfit.gameObject.SetActive(outfit.sprite);
        face.gameObject.SetActive(face.sprite);
        hairBack.gameObject.SetActive(hairBack.sprite);

        HidePortrait();
    }

    public void ShowPortrait(Mood expression = Mood.Neutral)
    {
        SetExpression(expression);
        gameObject.SetActive(true);
    }

    public void HidePortrait()
    {
        gameObject.SetActive(false);
    }
}
