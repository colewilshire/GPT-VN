using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitController : MonoBehaviour
{
    [SerializeField] private Image accessory;
    [SerializeField] private Image hairFront;
    [SerializeField] private Image outfit;
    [SerializeField] private Image face;
    [SerializeField] private Image hairBack;

    public void SetAppearance(CharacterAppearance characterPortrait)
    {
        accessory.sprite = characterPortrait.accessory;
        hairFront.sprite = characterPortrait.hairFront;
        outfit.sprite = characterPortrait.outfit;
        face.sprite = characterPortrait.face;
        hairBack.sprite = characterPortrait.hairBack;

        accessory.gameObject.SetActive(accessory.sprite);
        hairFront.gameObject.SetActive(hairFront.sprite);
        outfit.gameObject.SetActive(outfit.sprite);
        face.gameObject.SetActive(face.sprite);
        hairBack.gameObject.SetActive(hairBack.sprite);
    }
}
