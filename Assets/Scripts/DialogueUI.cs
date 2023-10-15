using System.Collections.Generic;

public class DialogueUI : Menu
{
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.Gameplay,
        GameState.Loading
    };
}
