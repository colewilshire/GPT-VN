using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        StateController.Instance.OnStateChange += OnStateChange;
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnDestroy()
    {
        StateController.Instance.OnStateChange -= OnStateChange;
        newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
        loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);
    }

    private void OnStateChange(GameState state)
    {
        gameObject.SetActive(state == GameState.PauseMenu);
    }

    private void OnNewGameButtonClicked()
    {
        OpenAIController.Instance.CreateNewConversation();
    }

    private void OnLoadGameButtonClicked()
    {
        StateController.Instance.SetState(GameState.LoadGameMenu);
    }

    private void OnQuitButtonClicked()
    {
        StateController.Instance.SetState(GameState.Gameplay);
    }
}
