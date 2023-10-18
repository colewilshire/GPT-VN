using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDisplay : MonoBehaviour
{
    [SerializeField] private Image screenshotDisplay;
    [SerializeField] private TMP_Text nameDisplay;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private async void OnClick()
    {
        if (await ConfirmationPrompt.Instance.PromptConfirmation("load this save?"))
        {
            OpenAIController.Instance.LoadConversationFromSave(nameDisplay.text);
        }
    }

    public void SetScreenshot(Sprite screenshot)
    {
        screenshotDisplay.sprite = screenshot;
    }

    public void SetNameDisplay(string name)
    {
        nameDisplay.text = name;
    }

    public void ShowDisplay()
    {
        gameObject.SetActive(true);
    }

    public void HideDisplay()
    {
        gameObject.SetActive(false);
    }
}
