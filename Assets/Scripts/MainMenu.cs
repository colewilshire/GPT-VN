using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        StateController.Instance.OnStateChange += OnStateChange;
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
    }

    private void OnDestroy()
    {
        StateController.Instance.OnStateChange -= OnStateChange;
        newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
        loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
    }

    private void OnStateChange(GameState state)
    {
        gameObject.SetActive(state == GameState.MainMenu);
    }

    private void OnNewGameButtonClicked()
    {
        OpenAIController.Instance.StartConversation();
    }

    private void OnLoadGameButtonClicked()
    {
        LoadGameMenu.Instance.ShowMenu();
    }
}
