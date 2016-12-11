using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField]
    public Menu CurrentMenu;

    [SerializeField]
    public EventSystem EventSystem;

    private bool buttonSelected = false;

    private void Start()
    {
        ShowMenu(CurrentMenu);
    }

//    private void Update()
//    {
//        if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
//        {
//            EventSystem.SetSelectedGameObject(CurrentMenu.FirstFocus);
//            buttonSelected = true;
//        }
//    }

    public void ShowMenu(Menu menu)
    {
        if (CurrentMenu != null) {
            CurrentMenu.IsOpen = false;
            EventSystem.SetSelectedGameObject(null);
        }

        if (menu == null)
        {
            buttonSelected = false;
            return;
        }

        CurrentMenu = menu;
        CurrentMenu.IsOpen = true;
        EventSystem.SetSelectedGameObject(CurrentMenu.FirstFocus);
    }

    public void ClearMenu()
    {
        if (CurrentMenu != null)
            CurrentMenu.IsOpen = false;

        CurrentMenu = null;
        buttonSelected = false;
        EventSystem.SetSelectedGameObject(null);
    }
}