using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSubmenu : Submenu
{
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.Gameplay,
        GameState.Loading
    };

    [Header("Navigation Buttons")]
    [SerializeField] private Button stepForwardButton;
    [SerializeField] private Button stepBackwardButton;
    [SerializeField] private Button repeatLineButton;

    [Header("Menu Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button settingsButton;

    protected override void Start()
    {
        base.Start();

        stepForwardButton.onClick.AddListener(DialogueController.Instance.StepForward);
        stepBackwardButton.onClick.AddListener(DialogueController.Instance.StepBackward);
        repeatLineButton.onClick.AddListener(DialogueController.Instance.RepeatLine);

        saveButton.onClick.AddListener(SaveController.Instance.Quicksave);
        loadButton.onClick.AddListener(() => StateController.Instance.SetState(GameState.LoadGameMenu));
        settingsButton.onClick.AddListener(() => StateController.Instance.SetState(GameState.MainMenu));
    }

    protected override void OnDestroy()
    {
        base.Start();

        stepForwardButton.onClick.AddListener(DialogueController.Instance.StepForward);
        stepBackwardButton.onClick.AddListener(DialogueController.Instance.StepBackward);
        repeatLineButton.onClick.AddListener(DialogueController.Instance.RepeatLine);

        saveButton.onClick.RemoveListener(SaveController.Instance.Quicksave);
        loadButton.onClick.RemoveListener(() => StateController.Instance.SetState(GameState.LoadGameMenu));
        settingsButton.onClick.RemoveListener(() => StateController.Instance.SetState(GameState.MainMenu));
    }

    // protected override void OnStateChange(GameState state)
    // {
    //     base.OnStateChange(state);

    //     bool isActiveState = ActiveStates.Contains(state);

    //     stepForwardButton.interactable = isActiveState;
    //     stepBackwardButton.interactable = isActiveState;
    //     repeatLineButton.interactable = isActiveState;

    //     saveButton.interactable = isActiveState;
    //     loadButton.interactable = isActiveState;
    //     settingsButton.interactable = isActiveState;
    // }
}
