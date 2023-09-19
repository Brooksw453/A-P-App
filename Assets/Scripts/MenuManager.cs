using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> menus;  // List of all your menus

    private List<GameObject> openedMenus = new List<GameObject>();

    private bool isToggled = false;

    private void Start()
{
    SetupInitialMenus();
}
    public void ToggleMenus()
    {
        if (isToggled)
        {
            // Reopen all previously opened menus
            foreach (GameObject menu in openedMenus)
            {
                menu.SetActive(true);
            }
            openedMenus.Clear();
        }
        else
        {
            // Store currently opened menus and then close them
            foreach (GameObject menu in menus)
            {
                if (menu.activeInHierarchy)
                {
                    openedMenus.Add(menu);
                    menu.SetActive(false);
                }
            }
        }
        
        isToggled = !isToggled;
    }

    // Called when your app starts to ensure the home menu is open and others are closed
    public void SetupInitialMenus()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);  // close all menus
        }
        
        menus[0].SetActive(true);  // assuming the home menu is the first in the list
    }
}

