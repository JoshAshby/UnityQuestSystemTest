using UnityEngine;
using Zenject;

[RequireComponent(typeof(MenuManager))]
public class InGameMenuManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField]
    private Menu PauseMenu = null;

    private MenuManager _menuManager;

    [Inject]
    private IGameManager _gameManager;

    private void Awake()
    {
        _menuManager = GetComponent<MenuManager>();
        _gameManager.fsm.OnStateChange += OnStateChange;
    }

    private void Destroy()
    {
        _gameManager.fsm.OnStateChange -= OnStateChange;
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
        _gameManager.State = GameStates.Playing;
    }

    public void QuitGame()
    {
        _gameManager.Quit();
    }
}