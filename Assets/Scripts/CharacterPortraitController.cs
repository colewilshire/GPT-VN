using System;
using System.Collections;
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

        accessory.sprite = characterAppearance.Accessory?.MainImage ?? null;
        hairFront.sprite = characterAppearance.Hair?.MainImage ?? null;
        outfit.sprite = characterAppearance.Outfit?.MainImage ?? null;
        face.sprite = characterAppearance.Face?.MainImage ?? null;
        hairBack.sprite = characterAppearance.Hair?.HairBackground ?? null;

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
        //UIEffectController.Instance.PlayEffect(gameObject, typeof(MovePortraitEffect));
        //UIEffectController.Instance.PlayEffect(gameObject, typeof(ScreenShakeEffect));
    }

    public void HidePortrait()
    {
        gameObject.SetActive(false);
    }
}
