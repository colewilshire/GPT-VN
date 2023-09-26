using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;

    private void Start()
    {
        StateController.Instance.OnStateChange += OnStateChange;
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
    }

    private void OnDestroy()
    {
        StateController.Instance.OnStateChange -= OnStateChange;
        newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
    }

    private void OnStateChange(GameState state)
    {
        gameObject.SetActive(state == GameState.MainMenu);
    }

    private void OnNewGameButtonClicked()
    {
        StateController.Instance.SetState(GameState.Gameplay);
    }
}
