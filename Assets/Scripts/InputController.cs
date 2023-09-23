using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputController : Singleton<InputController>
{
    private PlayerInputActions playerInputActions;
    private InputAction stepForwardAction;
    private InputAction stepBackwardAction;
    private InputAction repeatLineAction;
    private InputAction quicksaveAction;
    private InputAction quickloadAction;
    private bool inputEnabled = true;
    [SerializeField] private Button stepForwardButton;
    [SerializeField] private Button stepBackwardButton;
    [SerializeField] private Button repeatLineButton;

    protected override void Awake()
    {
        base.Awake();

        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        CreateInputs();
        SetUpButtons();
    }

    private void OnDestroy()
    {
        DestroyInputs();
        TearDownButtons();
    }

    private void CreateInputs()
    {
        stepForwardAction = playerInputActions.Dialogue.StepForward;
        stepBackwardAction = playerInputActions.Dialogue.StepBackward;
        repeatLineAction = playerInputActions.Dialogue.RepeatLine;
        quicksaveAction = playerInputActions.Dialogue.Quicksave;
        quickloadAction = playerInputActions.Dialogue.Quickload;

        stepForwardAction.performed += OnStepForward;
        stepBackwardAction.performed += OnStepBackward;
        repeatLineAction.performed += OnRepeatLine;
        quicksaveAction.performed += OnQuicksave;
        quickloadAction.performed += OnQuickload;

        stepForwardAction.Enable();
        stepBackwardAction.Enable();
        repeatLineAction.Enable();
        quicksaveAction.Enable();
        quickloadAction.Enable();
    }

    private void DestroyInputs()
    {
        stepForwardAction.performed -= OnStepForward;
        stepBackwardAction.performed -= OnStepBackward;
        repeatLineAction.performed -= OnRepeatLine;
        quicksaveAction.performed -= OnQuicksave;
        quickloadAction.performed -= OnQuickload;

        stepForwardAction.Disable();
        stepBackwardAction.Disable();
        repeatLineAction.Disable();
        quicksaveAction.Disable();
        quickloadAction.Disable();
    }

    private void SetUpButtons()
    {
        if (stepForwardButton)
        {
            stepForwardButton.onClick.AddListener(() => OnStepForward());
        }

        if (stepBackwardButton)
        {
            stepBackwardButton.onClick.AddListener(() => OnStepBackward());
        }

        if (repeatLineButton)
        {
            repeatLineButton.onClick.AddListener(() => OnRepeatLine());
        }
    }

    private void TearDownButtons()
    {
        if (stepForwardButton)
        {
            stepForwardButton.onClick.RemoveListener(() => OnStepForward());
        }

        if (stepBackwardButton)
        {
            stepBackwardButton.onClick.RemoveListener(() => OnStepBackward());
        }

        if (repeatLineButton)
        {
            repeatLineButton.onClick.RemoveListener(() => OnRepeatLine());
        }
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
