using System.Collections.Generic;

public class MainMenuSubmenu : Submenu
{
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.MainMenu
    };
}
