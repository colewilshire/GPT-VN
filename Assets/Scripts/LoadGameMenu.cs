using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameMenu : Menu
{
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.LoadGameMenu
    };

    private Dictionary<string, Sprite> screenshotDictionary;
    private int currentPageIndex = 0;
    [SerializeField] private List<SaveDisplay> saveDisplays;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button exitMenuButton;
    [SerializeField] private Scrollbar scrollbar;

    private void Awake()
    {
        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
        exitMenuButton.onClick.AddListener(ExitMenu);
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        nextPageButton.onClick.RemoveListener(NextPage);
        previousPageButton.onClick.RemoveListener(PreviousPage);
        exitMenuButton.onClick.RemoveListener(ExitMenu);
        scrollbar.onValueChanged.RemoveListener(OnScrollbarValueChanged);
    }

    private void OnScrollbarValueChanged(float value)
    {
        currentPageIndex = Mathf.RoundToInt(value * (scrollbar.numberOfSteps - 1));
        ShowPage(currentPageIndex);
    }

    protected override void ResetMenu()
    {
        base.ResetMenu();

        currentPageIndex = 0;
        screenshotDictionary = SaveController.Instance.GetSavesSortedByDate();
        scrollbar.numberOfSteps = (int)Math.Ceiling((double)screenshotDictionary.Count / saveDisplays.Count);
        ShowPage(0);
    }

    private void ShowPage(int pageIndex)
    {
        bool nextPageExists = (currentPageIndex + 1) * saveDisplays.Count < screenshotDictionary.Count;
        bool previousPageExists = currentPageIndex - 1 >= 0;

        nextPageButton.gameObject.SetActive(nextPageExists);
        previousPageButton.gameObject.SetActive(previousPageExists);

        for (int i = 0; i < saveDisplays.Count; ++i)
        {
            int saveSlotIndex = i + saveDisplays.Count * pageIndex;

            if (saveSlotIndex < screenshotDictionary.Count)
            {
                string saveName = screenshotDictionary.Keys.ElementAt(saveSlotIndex);
                Sprite screenshot = screenshotDictionary[saveName];

                saveDisplays[i].SetScreenshot(screenshot);
                saveDisplays[i].SetNameDisplay(saveName);
                saveDisplays[i].ShowDisplay();
            }
            else
            {
                saveDisplays[i].HideDisplay();
            }
        }
    }

    private void NextPage()
    {
        ++currentPageIndex;
        ShowPage(currentPageIndex);

        scrollbar.value = (float)currentPageIndex / (scrollbar.numberOfSteps - 1);
    }

    private void PreviousPage()
    {
        --currentPageIndex;
        ShowPage(currentPageIndex);

        scrollbar.value = (float)currentPageIndex / (scrollbar.numberOfSteps - 1);
    }

    public void ExitMenu()
    {
        StateController.Instance.SetState(StateController.Instance.PreviousState);
    }
}
