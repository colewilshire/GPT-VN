public abstract class Submenu : Menu
{
    protected override void Start()
    {
        StateController.Instance.OnSubmenuStateChange += OnStateChange;
    }

    protected override void OnDestroy()
    {
        StateController.Instance.OnSubmenuStateChange -= OnStateChange;
    }
}
