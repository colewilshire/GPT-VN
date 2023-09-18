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
                face.sprite = Appearance.face.image;
                break;
            case Mood.Sad:
                face.sprite = Appearance.face.image1 ?? Appearance.face.image;
                break;
            case Mood.Happy:
                face.sprite = Appearance.face.image2 ?? Appearance.face.image;
                break;
            case Mood.Angry:
                face.sprite = Appearance.face.image3 ?? Appearance.face.image;
                break;
            case Mood.Shocked:
                face.sprite = Appearance.face.image4 ?? Appearance.face.image;
                break;
            case Mood.Awkward:
                face.sprite = Appearance.face.image5 ?? Appearance.face.image;
                break;
            default:
                face.sprite = Appearance.face.image;
                break;
        }
    }

    public void SetAppearance(CharacterAppearance characterAppearance)
    {
        Appearance = characterAppearance;

        accessory.sprite = characterAppearance.accessory?.image ?? null;
        hairFront.sprite = characterAppearance.hair?.image ?? null;
        outfit.sprite = characterAppearance.outfit?.image ?? null;
        face.sprite = characterAppearance.face?.image ?? null;
        hairBack.sprite = characterAppearance.hair?.imageBackground ?? null;

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
