using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField]
    private Menu CurrentMenu;

    [SerializeField]
    private EventSystem EventSystem;

    private ILoadingScreenManager loadingScreenManager;

    private void Start()
    {
        ShowMenu(CurrentMenu);
    }

    public void ShowMenu(Menu menu)
    {
        if (CurrentMenu != null) {
            CurrentMenu.IsOpen = false;
            EventSystem.SetSelectedGameObject(null);
        }

        if (menu == null)
            return;

        CurrentMenu = menu;
        CurrentMenu.IsOpen = true;
        EventSystem.SetSelectedGameObject(CurrentMenu.FirstFocus);
    }

    public void ClearMenu()
    {
        if (CurrentMenu != null)
            CurrentMenu.IsOpen = false;

        CurrentMenu = null;
        EventSystem.SetSelectedGameObject(null);
    }

    public void LoadLevel(string levelName)
    {
        loadingScreenManager.LoadScene(levelName);
    }
}