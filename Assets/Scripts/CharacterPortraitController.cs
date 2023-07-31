using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitController : MonoBehaviour
{
    [SerializeField] private Image accessory;
    [SerializeField] private Image hairFront;
    [SerializeField] private Image outfit;
    [SerializeField] private Image face;
    [SerializeField] private Image hairBack;

    public void SetAppearance(CharacterAppearance characterAppearance)
    {
        accessory.sprite = characterAppearance.accessory;
        hairFront.sprite = characterAppearance.hairFront;
        outfit.sprite = characterAppearance.outfit;
        face.sprite = characterAppearance.face;
        hairBack.sprite = characterAppearance.hairBack;

        accessory.gameObject.SetActive(accessory.sprite);
        hairFront.gameObject.SetActive(hairFront.sprite);
        outfit.gameObject.SetActive(outfit.sprite);
        face.gameObject.SetActive(face.sprite);
        hairBack.gameObject.SetActive(hairBack.sprite);
    }
}
