using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Menu
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.PauseMenu
    };

    private void Awake()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
        optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnResumeButtonClicked()
    {
        StateController.Instance.SetState(GameState.Gameplay);
    }

    private void OnLoadGameButtonClicked()
    {
        StateController.Instance.SetSubmenuState(GameState.LoadGameMenu);
    }

    private void OnOptionsButtonClicked()
    {

    }

    private void OnMainMenuButtonClicked()
    {
        StateController.Instance.SetState(GameState.MainMenu);
    }
}
