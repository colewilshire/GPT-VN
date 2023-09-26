using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : Singleton<LoadingScreen>
{
    private TextMeshProUGUI loadingMessage;
    private ProgressBar progressBar;
    private Dictionary<LoadingState, string> loadingMessages = new Dictionary<LoadingState, string>
    {
        { LoadingState.Conversation, "Making first-contact..." },
        { LoadingState.Cast, "Casting characters..." },
        { LoadingState.Hair, "Styling hair..." },
        { LoadingState.Outfits, "Sewing outfits..." },
        { LoadingState.EyeColors, "Putting in colored contacts..." },
        { LoadingState.Accessories, "Accessorizing..." },
        { LoadingState.Backgrounds, "Hand-painting backgrounds..." },
        { LoadingState.Dialogue, "Writing screenplay..." },
        { LoadingState.AdditionalDialogue, "Publishing sequel direct-to-video..." }
    };
    private int statesEllapsed = 0;

    protected override void Awake()
    {
        base.Awake();

        loadingMessage = GetComponentInChildren<TextMeshProUGUI>();
        progressBar = GetComponentInChildren<ProgressBar>();
    }

    private void Start()
    {
        StateController.Instance.OnStateChange += OnStateChange;
    }

    private void OnDestroy()
    {
        StateController.Instance.OnStateChange -= OnStateChange;
    }

    private void OnStateChange(GameState state)
    {
        gameObject.SetActive(state == GameState.Loading);
        if (state != GameState.Loading) return;

        statesEllapsed = 0;
        SetLoadingState(LoadingState.Conversation);
    }

    public void SetLoadingState(LoadingState state)
    {
        ++statesEllapsed;
        loadingMessage.text = loadingMessages[state];

        float loadingProgress = (float) statesEllapsed / loadingMessages.Count;
        progressBar.SetProgress(loadingProgress);
    }
}
