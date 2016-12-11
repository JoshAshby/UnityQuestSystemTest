using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Menu CurrentMenu;

    public void Start()
    {
        ShowMenu(CurrentMenu);
    }

    public void ShowMenu(Menu menu)
    {
        if (CurrentMenu != null)
            CurrentMenu.IsOpen = false;

        if (menu == null)
            return;

        CurrentMenu = menu;
        CurrentMenu.IsOpen = true;
    }

    public void ClearMenu()
    {
        if (CurrentMenu != null)
            CurrentMenu.IsOpen = false;

        CurrentMenu = null;
    }
}