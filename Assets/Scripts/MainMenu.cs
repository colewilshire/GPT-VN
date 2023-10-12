using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private LoadGameMenu loadGameMenu;
    [SerializeField] private ParticleSystem mainMenuparticleSystem;
    protected override GameState activeState {get; set;} = GameState.MainMenu;

    private void Awake()
    {
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
    }

    protected override void Start()
    {
        loadGameMenu.CloseMenu();
        base.Start();
    }

    private void OnEnable()
    {
        mainMenuparticleSystem.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        mainMenuparticleSystem.gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
        loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
    }

    private void OnNewGameButtonClicked()
    {
        OpenAIController.Instance.CreateNewConversation();
    }

    private void OnLoadGameButtonClicked()
    {
        buttons.SetActive(false);
        loadGameMenu.OpenMenu();
    }
}
