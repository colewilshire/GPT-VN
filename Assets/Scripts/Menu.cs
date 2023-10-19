using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    private readonly Dictionary<GameObject, bool> defaultChildStates = new();
    protected abstract HashSet<GameState> ActiveStates { get; set; }

    protected virtual void Start()
    {
        StateController.Instance.OnStateChange += OnStateChange;

        // foreach (Transform child in transform)
        // {
        //     defaultChildStates[child.gameObject] = child.gameObject.activeSelf;
        // }
    }

    protected virtual void OnDestroy()
    {
        StateController.Instance.OnStateChange -= OnStateChange;
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
        foreach (KeyValuePair<GameObject, bool> entry in defaultChildStates)
        {
            entry.Key.SetActive(entry.Value);
        }
    }
}
