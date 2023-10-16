using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPrompt : Singleton<ConfirmationPrompt>
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private TMP_Text promptText;
    private TaskCompletionSource<bool> confirmed;

    protected override void Awake()
    {
        base.Awake();
        yesButton.onClick.AddListener(YesButtonPressed);
        noButton.onClick.AddListener(NoButtonPressed);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        yesButton.onClick.RemoveListener(YesButtonPressed);
        noButton.onClick.RemoveListener(NoButtonPressed);
    }

    private void YesButtonPressed()
    {
        gameObject.SetActive(false);
        confirmed.SetResult(true);
    }

    private void NoButtonPressed()
    {
        gameObject.SetActive(false);
        confirmed.SetResult(false);
    }

    public async Task<bool> PromptConfirmation(string confirmationText)
    {
        confirmed = new TaskCompletionSource<bool>();

        promptText.text = confirmationText;
        gameObject.SetActive(true);

        if (await confirmed.Task == true)
        {
            return true;
        }

        return false;
    }
}
