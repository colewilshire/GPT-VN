using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameSubmenu : Submenu
{
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.LoadGameMenu
    };

    private Dictionary<string, Sprite> screenshotDictionary;
    private int currentPageIndex = 0;
    private bool deletionEnabled = false;

    [SerializeField] private List<SaveDisplay> saveDisplays;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button exitMenuButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Scrollbar scrollbar;

    private void Awake()
    {
        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
        exitMenuButton.onClick.AddListener(ExitMenu);
        deleteButton.onClick.AddListener(EnableSaveDeletion);
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        nextPageButton.onClick.RemoveListener(NextPage);
        previousPageButton.onClick.RemoveListener(PreviousPage);
        exitMenuButton.onClick.RemoveListener(ExitMenu);
        deleteButton.onClick.RemoveListener(EnableSaveDeletion);
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
        deletionEnabled = false;

        EnableSaveDeletion();
        ReloadPage();
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

            if (screenshotDictionary.Count > 0 && saveSlotIndex < screenshotDictionary.Count)
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

    private void EnableSaveDeletion()
    {
        foreach (SaveDisplay saveDisplay in saveDisplays)
        {
            saveDisplay.EnableDeleteButton(deletionEnabled);
        }

        deletionEnabled = !deletionEnabled;
    }

    public void ReloadPage()
    {
        screenshotDictionary = SaveController.Instance.GetSavesSortedByDate();
        scrollbar.numberOfSteps = (int)Math.Ceiling((double)screenshotDictionary.Count / saveDisplays.Count);

        if (scrollbar.numberOfSteps > 1)
        {
            scrollbar.value = (float)currentPageIndex / (scrollbar.numberOfSteps - 1);
        }
        else
        {
            scrollbar.value = 0;
        }

        ShowPage(currentPageIndex);
    }
}
