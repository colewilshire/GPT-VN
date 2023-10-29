using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    private readonly Dictionary<GameObject, bool> defaultChildStates = new();
    protected abstract HashSet<GameState> ActiveStates { get; set; }

    protected virtual void Start()
    {
        StateController.Instance.OnMenuStateChange += OnStateChange;
    }

    protected virtual void OnDestroy()
    {
        StateController.Instance.OnMenuStateChange -= OnStateChange;
    }

    protected virtual void OnStateChange(GameState state)
    {
        bool isActiveState = ActiveStates.Contains(state);

        gameObject.SetActive(isActiveState);
        if (!isActiveState) return;

        ResetMenu();
    }

    protected virtual void ResetMenu()
    {

    }

    protected virtual void ExitMenu()
    {
        StateController.Instance.ReturnToPreviousStates();
    }
}
