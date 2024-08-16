using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDisplay : MonoBehaviour
{
    [SerializeField] private Image screenshotDisplay;
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private Button loadSaveButton;
    [SerializeField] private Button deleteSaveButton;
    private LoadGameSubmenu loadGameSubmenu;

    private void Awake()
    {
        loadSaveButton.onClick.AddListener(LoadSave);
        deleteSaveButton.onClick.AddListener(DeleteSave);
    }

    private void OnDestroy()
    {
        loadSaveButton.onClick.RemoveListener(LoadSave);
        deleteSaveButton.onClick.RemoveListener(DeleteSave);
    }

    private void Start()
    {
        loadGameSubmenu = gameObject.GetComponentInParent<LoadGameSubmenu>();
    }

    private async void LoadSave()
    {
        if (await ConfirmationPrompt.Instance.PromptConfirmation("load this save?"))
        {
            OpenAIController.Instance.LoadConversationFromSave(nameDisplay.text);
        }
    }

    private async void DeleteSave()
    {
        if (await ConfirmationPrompt.Instance.PromptConfirmation("delete this save?"))
        {
            SaveController.Instance.DeleteSave(nameDisplay.text);
            loadGameSubmenu.ReloadPage();
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

    public void EnableDeleteButton(bool isEnabled)
    {
        deleteSaveButton.gameObject.SetActive(isEnabled);
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
