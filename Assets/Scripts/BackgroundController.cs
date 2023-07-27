using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : Singleton<BackgroundController>
{
    private Image backgroundImage;

    protected override void Awake()
    {
        base.Awake();

        backgroundImage = GetComponent<Image>();
    }

    public void SetBackground(Sprite sprite)
    {
        if (!sprite) return;
        backgroundImage.sprite = sprite;
    }
}
