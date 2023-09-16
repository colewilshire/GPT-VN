using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitController : MonoBehaviour
{
    [SerializeField] private Image accessory;
    [SerializeField] private Image hairFront;
    [SerializeField] private Image outfit;
    [SerializeField] private Image face;
    [SerializeField] private Image hairBack;
    private CharacterAppearance appearance;

    public void SetAppearance(CharacterAppearance characterAppearance)
    {
        appearance = characterAppearance;

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

    public void SetExpression(Mood expression)
    {
        Sprite newFace = appearance.face.image1;
        face.sprite = newFace;
    }

    public void ShowPortrait()
    {
        gameObject.SetActive(true);
    }

    public void HidePortrait()
    {
        gameObject.SetActive(false);
    }
}
