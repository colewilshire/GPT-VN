using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPrompt : Singleton<InputPrompt>
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text prompt;
    [SerializeField] private TMP_InputField inputField;
    private TaskCompletionSource<bool> confirmed;

    protected override void Awake()
    {
        base.Awake();
        confirmButton.onClick.AddListener(ConfirmButtonPressed);
        backButton.onClick.AddListener(BackButtonPressed);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        confirmButton.onClick.RemoveListener(ConfirmButtonPressed);
        backButton.onClick.RemoveListener(BackButtonPressed);
    }

    private void ConfirmButtonPressed()
    {
        gameObject.SetActive(false);
        confirmed.SetResult(true);
    }

    private void BackButtonPressed()
    {
        gameObject.SetActive(false);
        confirmed.SetResult(false);
    }

    public async Task<string> PromptInput(string promptText, string placeholderText)
    {
        TextMeshProUGUI placeholder = (TextMeshProUGUI) inputField.placeholder;
        confirmed = new TaskCompletionSource<bool>();

        inputField.text = "";
        prompt.text = promptText;
        placeholder.text = placeholderText;
        InputController.Instance.DisableInputs();
        gameObject.SetActive(true);

        bool isPromptConfirmed = await confirmed.Task;
        InputController.Instance.EnableInputs();

        if (isPromptConfirmed == false) return null;
        return !string.IsNullOrEmpty(inputField.text) ? inputField.text : placeholderText;
    }
}
