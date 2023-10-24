public class StateController : Singleton<StateController>
{
    private readonly SingletonStack<GameState> menuStates = new();
    private readonly SingletonStack<GameState> submenuStates = new();
    private readonly GameState defaultState = GameState.MainMenu;

    public delegate void OnStateChangeHandler(GameState state);
    public delegate void OnSubmenuStateChangeHandler(GameState state);
    public event OnStateChangeHandler OnMenuStateChange;
    public event OnSubmenuStateChangeHandler OnSubmenuStateChange;

    private void Start()
    {
        SetStates(defaultState);
    }

    public void SetStates(GameState state)
    {
        SetMenuState(state);
        SetSubmenuState(state);
    }

    public void SetMenuState(GameState state)
    {
        menuStates.Push(state);
        OnMenuStateChange?.Invoke(state);
    }

    public void SetSubmenuState(GameState state)
    {
        submenuStates.Push(state);
        OnSubmenuStateChange?.Invoke(state);
    }

    public void ReturnToPreviousStates()
    {
        ReturnToPreviousMenuState();
        ReturnToPreviousSubmenuState();
    }

    public void ReturnToPreviousMenuState()
    {
        if (menuStates.TryPop(out GameState currentState) && menuStates.TryPeek(out GameState previousState))
        {
            OnMenuStateChange?.Invoke(previousState);
        }
    }

    public void ReturnToPreviousSubmenuState()
    {
        if (submenuStates.TryPop(out GameState currentState) && submenuStates.TryPeek(out GameState previousState))
        {
            OnSubmenuStateChange?.Invoke(previousState);
        }
    }
}
