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

    private void ResetMenu()
    {
        currentPageIndex = 0;
        screenshotDictionary = SaveController.Instance.GetSavesSortedByDate();
        ShowPage(0);
    }

    private void ShowPage(int pageIndex)
    {
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
        if (!(currentPageIndex * saveDisplays.Count + 1 < screenshotDictionary.Count)) return;

        ++currentPageIndex;
        ShowPage(currentPageIndex);
    }

    private void PreviousPage()
    {
        if (!(currentPageIndex - 1 >= 0)) return;

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
}
