using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameMenu : MonoBehaviour
{
    private Dictionary<string, Sprite> screenshotDictionary;
    private int currentPageIndex = 0;
    [SerializeField] private List<SaveDisplay> saveDisplays;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button exitMenuButton;

    private void Awake()
    {
        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
        exitMenuButton.onClick.AddListener(ExitMenu);
    }

    private void OnDestroy()
    {
        nextPageButton.onClick.RemoveListener(NextPage);
        previousPageButton.onClick.RemoveListener(PreviousPage);
        exitMenuButton.onClick.RemoveListener(ExitMenu);
    }

    private void ResetMenu()
    {
        currentPageIndex = 0;
        screenshotDictionary = SaveController.Instance.GetSavesSortedByDate();
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
    }

    private void PreviousPage()
    {
        --currentPageIndex;
        ShowPage(currentPageIndex);
    }

    public void OpenMenu()
    {
        ResetMenu();
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void ExitMenu()
    {
        StateController.Instance.SetState(StateController.Instance.CurrentState);
    }
}
