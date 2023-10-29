using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortrait : MonoBehaviour
{
    [SerializeField] private Image accessory;
    [SerializeField] private Image hairFront;
    [SerializeField] private Image outfit;
    [SerializeField] private Image face;
    [SerializeField] private Image hairBack;
    [SerializeField] private Image head;
    public CharacterAppearance Appearance;

    [SerializeField] private Accessory accessoryScriptableObject;
    [SerializeField] private Hair hairScriptableObject;
    [SerializeField] private Outfit outfitScriptableObject;
    [SerializeField] private Face faceScriptableObject;

    public string DisplayName = "";

    private void SetExpression(Mood expression)
    {
        if(!face.sprite) return;

        switch (expression)
        {
            case Mood.Neutral:
                face.sprite = Appearance.Face.MainImage;
                break;
            case Mood.Sad:
                face.sprite = Appearance.Face.Sad != null ? Appearance.Face.Sad : Appearance.Face.MainImage;
                break;
            case Mood.Happy:
                face.sprite = Appearance.Face.Happy != null ? Appearance.Face.Happy : Appearance.Face.MainImage;
                break;
            case Mood.Angry:
                face.sprite = Appearance.Face.Angry != null ? Appearance.Face.Angry : Appearance.Face.MainImage;
                break;
            case Mood.Shocked:
                face.sprite = Appearance.Face.Shocked != null ? Appearance.Face.Shocked : Appearance.Face.MainImage;
                break;
            case Mood.Awkward:
                face.sprite = Appearance.Face.Awkward != null ? Appearance.Face.Awkward : Appearance.Face.MainImage;
                break;
            default:
                face.sprite = Appearance.Face.MainImage;
                break;
        }
    }

    public void SetAppearance(CharacterAppearance characterAppearance)
    {
        Appearance = characterAppearance;
        accessoryScriptableObject = characterAppearance.Accessory;
        outfitScriptableObject = characterAppearance.Outfit;
        hairScriptableObject = characterAppearance.Hair;
        faceScriptableObject = characterAppearance.Face;

        accessory.sprite = characterAppearance.Accessory != null ? characterAppearance.Accessory.MainImage : null;
        hairFront.sprite = characterAppearance.Hair != null ? characterAppearance.Hair.MainImage : null;
        outfit.sprite = characterAppearance.Outfit != null ? characterAppearance.Outfit.MainImage : null;
        face.sprite = characterAppearance.Face != null ? characterAppearance.Face.MainImage : null;
        hairBack.sprite = characterAppearance.Hair != null ? characterAppearance.Hair.HairBackground : null;

        accessory.gameObject.SetActive(accessory.sprite);
        hairFront.gameObject.SetActive(hairFront.sprite);
        outfit.gameObject.SetActive(outfit.sprite);
        face.gameObject.SetActive(face.sprite);
        hairBack.gameObject.SetActive(hairBack.sprite);
        head.gameObject.SetActive(face.sprite);

        HidePortrait();
    }

    public void SetAccessory(Accessory newAccessory)
    {
        accessory.sprite = newAccessory.MainImage;
        accessory.gameObject.SetActive(accessory.sprite);
    }

    public void SetHair(Hair newHair)
    {
        hairFront.sprite = newHair.MainImage;
        hairBack.sprite = newHair.HairBackground;
        hairFront.gameObject.SetActive(hairFront.sprite);
        hairBack.gameObject.SetActive(hairBack.sprite);
    }

    public void SetOutfit(Outfit newOutfit)
    {
        outfit.sprite = newOutfit.MainImage;
        outfit.gameObject.SetActive(outfit.sprite);
    }

    public void SetFace(Face newFace)
    {
        face.sprite = newFace.MainImage;
        face.gameObject.SetActive(face.sprite);
    }

    public void ShowPortrait(Mood expression = Mood.Neutral)
    {
        SetExpression(expression);
        gameObject.SetActive(true);
        //UIEffectController.Instance.PlayEffect(gameObject, typeof(MovePortraitEffect));
        //UIEffectController.Instance.PlayEffect(gameObject, typeof(ScreenShakeEffect));
    }

    public void HidePortrait()
    {
        gameObject.SetActive(false);
    }
}
