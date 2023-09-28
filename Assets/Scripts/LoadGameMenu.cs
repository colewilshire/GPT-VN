using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameMenu : Singleton<LoadGameMenu>
{
    private Dictionary<string, Sprite> screenshotDictionary;
    private int currentPageIndex = 0;
    [SerializeField] private List<Image> saveDisplays;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;

    protected override void Awake()
    {
        base.Awake();

        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
    }

    private void Start()
    {
        StateController.Instance.OnStateChange += OnStateChange;
        HideMenu();
    }

    private void OnDestroy()
    {
        nextPageButton.onClick.RemoveListener(NextPage);
        previousPageButton.onClick.RemoveListener(PreviousPage);
        StateController.Instance.OnStateChange -= OnStateChange;
    }

    private void OnStateChange(GameState state)
    {
        if (state != GameState.MainMenu) return;
        ResetMenu();
    }

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

                saveDisplays[i].sprite = screenshot;
                saveDisplays[i].name = saveName;
                saveDisplays[i].gameObject.SetActive(true);
            }
            else
            {
                saveDisplays[i].gameObject.SetActive(false);
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

    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
}
