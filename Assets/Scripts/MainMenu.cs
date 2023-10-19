using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.MainMenu
    };

    private void Awake()
    {
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
        loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
    }

    private void OnNewGameButtonClicked()
    {
        OpenAIController.Instance.CreateNewConversation();
    }

    private void OnLoadGameButtonClicked()
    {
        StateController.Instance.SetSubmenuState(GameState.LoadGameMenu);
    }
}
