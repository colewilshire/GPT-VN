using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : Singleton<LoadingScreen>
{
    [SerializeField] private TextMeshProUGUI loadingMessage;
    [SerializeField] private ProgressBar progressBar;
    private readonly Dictionary<LoadingState, List<string>> loadingMessages = new()
    {
        { LoadingState.Initial, new List<string>()
            {
                "making first contact...",
                "casting characters...",
                "writing screenplay..."
            }
        },
        { LoadingState.Additional, new List<string>()
            {
                "publishing direct-to-video sequel...",
                "failing financially..."
            }
        }
    };
    private List<string> currentLoadingMessages;
    private int currentIndex = 0;

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

    public void SetLoadingState(LoadingState state, int currentIndex = 0)
    {
        currentLoadingMessages = loadingMessages[state];
        loadingMessage.text = currentLoadingMessages[currentIndex];

        float loadingProgress = (float) (currentIndex + 1) / currentLoadingMessages.Count;
        progressBar.SetProgress(loadingProgress);
    }

    public void StartLoading(LoadingState state)
    {
        currentIndex = 0;
        currentLoadingMessages = loadingMessages[state];

        progressBar.SetProgress(0, 0);
        IncrementLoadingMessage();
    }

    public void IncrementLoadingMessage()
    {
        currentIndex = currentIndex < currentLoadingMessages.Count ? currentIndex : currentLoadingMessages.Count;
        loadingMessage.text = currentLoadingMessages[currentIndex];

        float loadingProgress = (float) (currentIndex + 1) / currentLoadingMessages.Count;
        progressBar.SetProgress(loadingProgress);

        currentIndex++;
    }
}
