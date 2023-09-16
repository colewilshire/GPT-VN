using System.Collections.Generic;
using TMPro;

public class LoadingScreen : Singleton<LoadingScreen>
{
    private TextMeshProUGUI loadingMessage;
    Dictionary<LoadingState, string> loadingMessages = new Dictionary<LoadingState, string>
    {
        { LoadingState.Conversation, "Making first-contact..." },
        { LoadingState.Cast, "Casting characters..." },
        { LoadingState.Hair, "Styling hair..." },
        { LoadingState.Outfits, "Sewing outfits..." },
        { LoadingState.EyeColors, "Putting-in colored contacts..." },
        { LoadingState.Accessories, "Accessorizing..." },
        { LoadingState.Backgrounds, "Hand-painting backgrounds..." },
        { LoadingState.Dialogue, "Writing screenplay..." },
        { LoadingState.AdditionalDialogue, "Publishing sequel direct-to-video..." }
    };

    protected override void Awake()
    {
        base.Awake();

        loadingMessage = GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }

    public void SetLoadingMessage(LoadingState state)
    {
        loadingMessage.text = loadingMessages[state];
    }

    public void ShowLoadingScreen()
    {
        InputController.Instance.DisableInputs();
        SetLoadingMessage(LoadingState.Conversation);
        gameObject.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        gameObject.SetActive(false);
        InputController.Instance.EnableInputs();
    }
}
