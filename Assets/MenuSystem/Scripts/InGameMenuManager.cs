using UnityEngine;
using Zenject;

public class InGameMenuManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField]
    public Menu PauseMenu = null;

    [Inject]
    private IGameManager _gameManager;

    private MenuManager _menuManager;

    private void Awake()
    {
        _menuManager = GetComponent<MenuManager>();
    }

    private void Update()
    {
        if (_gameManager.fsm.CurrentState == GameStates.Menu)
            _menuManager.ShowMenu(PauseMenu);
        else
            _menuManager.ClearMenu();
    }

    public void ResumeGame()
    {
        _gameManager.SetTransition(GameStates.Playing);
    }

    public void QuitGame()
    {
        _gameManager.Quit();
    }
}