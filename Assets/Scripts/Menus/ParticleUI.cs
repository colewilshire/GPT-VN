using System.Collections.Generic;

public class ParticleUI : Menu
{
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.MainMenu,
        GameState.Loading
    };
}
