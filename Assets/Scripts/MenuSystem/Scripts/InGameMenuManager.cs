using UnityEngine;

[RequireComponent(typeof(MenuManager))]
public class InGameMenuManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField]
    private Menu PauseMenu = null;

    private MenuManager _menuManager;

    private void Awake()
    {
        _menuManager = GetComponent<MenuManager>();
        GameManager.Instance.fsm.OnStateChange += OnStateChange;
    }

    private void Destroy()
    {
        GameManager.Instance.fsm.OnStateChange -= OnStateChange;
    }

    public void OnStateChange(GameStates state)
    {
        if (state == GameStates.Menu)
            _menuManager.ShowMenu(PauseMenu);
        else
            _menuManager.ClearMenu();
    }

    public void ResumeGame()
    {
        GameManager.Instance.State = GameStates.Playing;
    }

    public void QuitGame()
    {
        GameManager.Instance.Quit();
    }
}