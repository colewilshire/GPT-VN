using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : Singleton<LoadingScreen>
{
    [SerializeField] private TextMeshProUGUI loadingMessage;
    [SerializeField] private ProgressBar progressBar;
    private readonly Dictionary<LoadingState, string> loadingMessages = new()
    {
        { LoadingState.Conversation, "making first contact..." },
        { LoadingState.Cast, "casting characters..." },
        // { LoadingState.Hair, "styling hair..." },
        // { LoadingState.Outfits, "sewing outfits..." },
        // { LoadingState.EyeColors, "putting in colored contacts..." },
        // { LoadingState.Accessories, "accessorizing..." },
        // { LoadingState.Backgrounds, "hand-painting backgrounds..." },
        { LoadingState.Dialogue, "writing screenplay..." },
        { LoadingState.AdditionalDialogue, "publishing direct-to-video sequel..." }
    };
    private int statesEllapsed = 0;

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

        statesEllapsed = 0;
        progressBar.SetProgress(0, 0);
    }

    public void SetLoadingState(LoadingState state)
    {
        ++statesEllapsed;
        loadingMessage.text = loadingMessages[state];

        float loadingProgress = (float) statesEllapsed / (loadingMessages.Count - 1);
        progressBar.SetProgress(loadingProgress);
    }
}
