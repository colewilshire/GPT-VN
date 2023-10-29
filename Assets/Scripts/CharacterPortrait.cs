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
    public CharacterAppearance Appearance { get; private set; }

    [SerializeField] private Accessory accessoryScriptableObject;
    [SerializeField] private Hair hairScriptableObject;
    [SerializeField] private Outfit outfitScriptableObject;
    [SerializeField] private Face faceScriptableObject;

    private void SetExpression(Mood expression)
    {
        if(!face.sprite) return;

        switch (expression)
        {
            case Mood.Neutral:
                face.sprite = Appearance.Face.MainImage;
                break;
            case Mood.Sad:
                face.sprite = Appearance.Face.Sad ?? Appearance.Face.MainImage;
                break;
            case Mood.Happy:
                face.sprite = Appearance.Face.Happy ?? Appearance.Face.MainImage;
                break;
            case Mood.Angry:
                face.sprite = Appearance.Face.Angry ?? Appearance.Face.MainImage;
                break;
            case Mood.Shocked:
                face.sprite = Appearance.Face.Shocked ?? Appearance.Face.MainImage;
                break;
            case Mood.Awkward:
                face.sprite = Appearance.Face.Awkward ?? Appearance.Face.MainImage;
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

        accessory.sprite = characterAppearance.Accessory.MainImage;
        hairFront.sprite = characterAppearance.Hair.MainImage;
        outfit.sprite = characterAppearance.Outfit.MainImage;
        face.sprite = characterAppearance.Face.MainImage;
        hairBack.sprite = characterAppearance.Hair.HairBackground;

        accessory.gameObject.SetActive(accessory.sprite);
        hairFront.gameObject.SetActive(hairFront.sprite);
        outfit.gameObject.SetActive(outfit.sprite);
        face.gameObject.SetActive(face.sprite);
        hairBack.gameObject.SetActive(hairBack.sprite);

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
