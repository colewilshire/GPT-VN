using TMPro;

public class LoadingScreen : Singleton<LoadingScreen>
{
    private TextMeshProUGUI loadingMessage;

    protected override void Awake()
    {
        base.Awake();

        loadingMessage = GetComponentInChildren<TextMeshProUGUI>();

        gameObject.SetActive(false);
    }

    public void SetLoadingMessage(string message = "")
    {
        loadingMessage.text = message;
    }

    public void ShowLoadingScreen(string message = "")
    {
        InputController.Instance.DisableInputs();
        SetLoadingMessage(message);
        gameObject.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        gameObject.SetActive(false);
        InputController.Instance.EnableInputs();
    }
}
