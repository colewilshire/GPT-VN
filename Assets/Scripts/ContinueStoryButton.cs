using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueStoryButton : Singleton<ContinueStoryButton>
{
    private Button button;

    protected override void Awake()
    {
        base.Awake();

        button = GetComponent<Button>();

        HideButton();
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        HideButton();
        OpenAIController.Instance.GenerateAdditionalDialogue();
    }

    public void ShowButton()
    {
        gameObject.SetActive(true);
    }

    public void HideButton()
    {
        gameObject.SetActive(false);
    }
}
