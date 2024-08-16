using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : Singleton<LoadingScreen>
{
    [SerializeField] private TextMeshProUGUI loadingMessage;
    [SerializeField] private ProgressBar progressBar;
    private readonly Dictionary<LoadingState, List<string>> loadingMessages = new()
    {
        { LoadingState.Conversation, new List<string>()
            {
                "making first contact...",
                "casting characters...",
                "writing screenplay..."
            }
        },
        { LoadingState.AdditionalDialogue, new List<string>()
            {
                "publishing direct-to-video sequel...",
                "failing financially..."
            }
        }
    };
    private List<string> currentLoadingMessages;

    private void Start()
    {
        StateController.Instance.OnMenuStateChange += OnStateChange;
    }

    private void OnDestroy()
    {
        StateController.Instance.OnMenuStateChange -= OnStateChange;
    }

    private void OnStateChange(GameState state)
    {
        gameObject.SetActive(state == GameState.Loading);
        if (state != GameState.Loading) return;

        progressBar.SetProgress(0, 0);
    }

    public void SetLoadingState(LoadingState state, int index = 0)
    {
        currentLoadingMessages = loadingMessages[state];
        loadingMessage.text = currentLoadingMessages[index];

        float loadingProgress = (float) (index + 1) / currentLoadingMessages.Count;
        progressBar.SetProgress(loadingProgress);
    }
}
