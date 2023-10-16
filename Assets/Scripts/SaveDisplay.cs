using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveDisplay : MonoBehaviour
{
    private Image screenshotDisplay;
    private Button button;
    private TMP_Text nameDisplay;

    private void Awake()
    {
        screenshotDisplay = GetComponent<Image>();
        button = GetComponent<Button>();
        nameDisplay = GetComponentInChildren<TMP_Text>();

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
