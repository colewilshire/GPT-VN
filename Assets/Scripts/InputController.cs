using UnityEngine.InputSystem;

public class InputController : Singleton<InputController>
{
    private PlayerInputActions playerInputActions;

    private InputAction stepForwardAction;
    private InputAction stepBackwardAction;
    private InputAction repeatLineAction;
    private InputAction quicksaveAction;
    private InputAction quickloadAction;
    private InputAction pauseAction;

    private bool inputEnabled = false;

    protected override void Awake()
    {
        base.Awake();

        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        StateController.Instance.OnMenuStateChange += OnStateChange;

        CreateInputs();
    }

    private void OnDestroy()
    {
        StateController.Instance.OnMenuStateChange -= OnStateChange;

        DestroyInputs();
    }

    private void OnStateChange(GameState state)
    {
        if (state == GameState.Gameplay)
        {
            EnableInputs();
        }
        else
        {
            DisableInputs();
        }
    }

    private void CreateInputs()
    {
        stepForwardAction = playerInputActions.Dialogue.StepForward;
        stepBackwardAction = playerInputActions.Dialogue.StepBackward;
        repeatLineAction = playerInputActions.Dialogue.RepeatLine;
        quicksaveAction = playerInputActions.Dialogue.Quicksave;
        quickloadAction = playerInputActions.Dialogue.Quickload;
        pauseAction = playerInputActions.Dialogue.Pause;

        stepForwardAction.performed += OnStepForward;
        stepBackwardAction.performed += OnStepBackward;
        repeatLineAction.performed += OnRepeatLine;
        quicksaveAction.performed += OnQuicksave;
        quickloadAction.performed += OnQuickload;
        pauseAction.performed += OnPause;

        stepForwardAction.Enable();
        stepBackwardAction.Enable();
        repeatLineAction.Enable();
        quicksaveAction.Enable();
        quickloadAction.Enable();
        pauseAction.Enable();
    }

    private void DestroyInputs()
    {
        stepForwardAction.performed -= OnStepForward;
        stepBackwardAction.performed -= OnStepBackward;
        repeatLineAction.performed -= OnRepeatLine;
        quicksaveAction.performed -= OnQuicksave;
        quickloadAction.performed -= OnQuickload;
        pauseAction.performed -= OnPause;

        stepForwardAction.Disable();
        stepBackwardAction.Disable();
        repeatLineAction.Disable();
        quicksaveAction.Disable();
        quickloadAction.Disable();
        pauseAction.Disable();
    }

    private void OnStepForward(InputAction.CallbackContext context = default)
    {
        if (!inputEnabled) return;
        DialogueController.Instance.StepForward();
    }

    private void OnStepBackward(InputAction.CallbackContext context = default)
    {
        if (!inputEnabled) return;
        DialogueController.Instance.StepBackward();
    }

    private void OnRepeatLine(InputAction.CallbackContext context = default)
    {
        if (!inputEnabled) return;
        DialogueController.Instance.RepeatLine();
    }

    private void OnQuicksave(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
        SaveController.Instance.Quicksave();
    }

    private void OnQuickload(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
        SaveController.Instance.Quickload();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
        StateController.Instance.SetStates(GameState.PauseMenu);
    }

    public void EnableInputs()
    {
        inputEnabled = true;
        playerInputActions.Enable();
    }

    public void DisableInputs()
    {
        inputEnabled = false;
        playerInputActions.Disable();
    }
}
