using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSubmenu : Submenu
{
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.MainMenu
    };

    protected override void Start()
    {
        base.Start();

        StateController.Instance.OnStateChange += OnStateChange;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        StateController.Instance.OnStateChange -= OnStateChange;
    }
}
