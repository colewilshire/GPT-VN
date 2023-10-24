public class StateController : Singleton<StateController>
{
    private readonly GameState defaultState = GameState.MainMenu;

    public delegate void OnStateChangeHandler(GameState state);
    public delegate void OnSubmenuStateChangeHandler(GameState state);
    public event OnStateChangeHandler OnStateChange;
    public event OnSubmenuStateChangeHandler OnSubmenuStateChange;

    public GameState PreviousState;// { get; private set; }
    public GameState PreviousSubmenuState;// { get; private set; }
    public GameState CurrentMenuState;// { get; private set; }
    public GameState CurrentSubmenuState;// { get; private set; }

    private void Start()
    {
        SetAllStates(defaultState);
    }

    public void SetAllStates(GameState state)
    {
        SetMenuState(state);
        SetSubmenuState(state);
    }

    public void SetMenuState(GameState state)
    {
        PreviousState = CurrentMenuState;
        CurrentMenuState = state;
        OnStateChange?.Invoke(state);
    }

    public void SetSubmenuState(GameState state)
    {
        PreviousSubmenuState = CurrentSubmenuState;
        CurrentSubmenuState = state;
        OnSubmenuStateChange?.Invoke(state);
    }
}