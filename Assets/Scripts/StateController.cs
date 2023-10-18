public class StateController : Singleton<StateController>
{
    private readonly GameState defaultState = GameState.MainMenu;
    public delegate void OnStateChangeHandler(GameState state);
    public event OnStateChangeHandler OnStateChange;
    public GameState PreviousState { get; private set; }
    public GameState CurrentState { get; private set; }

    private void Start()
    {
        SetState(defaultState);
    }

    public void SetState(GameState state)
    {
        PreviousState = CurrentState;
        CurrentState = state;
        OnStateChange?.Invoke(state);
    }
}