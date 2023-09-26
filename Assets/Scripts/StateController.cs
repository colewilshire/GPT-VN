public class StateController : Singleton<StateController>
{
    private GameState defaultState = GameState.MainMenu;
    public delegate void OnStateChangeHandler(GameState state);
    public event OnStateChangeHandler OnStateChange;
    public GameState CurrentState { get; private set; }

    private void Start()
    {
        SetState(defaultState);
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
        OnStateChange?.Invoke(state);
    }
}