using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> menus;
    [SerializeField] private int homeMenuIndex = 0;

    private List<GameObject> openedMenus = new List<GameObject>();
    private bool isToggled = false;
    private int currentMenuIndex = -1;

    private void Start()
    {
        SetupInitialMenus();
    }

    public void ToggleMenus()
    {
        if (isToggled)
        {
            foreach (GameObject menu in openedMenus)
            {
                menu.SetActive(true);
            }
            openedMenus.Clear();
        }
        else
        {
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

    public void ShowMenu(int index)
    {
        if (index < 0 || index >= menus.Count) return;

        for (int i = 0; i < menus.Count; i++)
        {
            menus[i].SetActive(i == index);
        }
        currentMenuIndex = index;
    }

    public void ShowMenu(GameObject menu)
    {
        foreach (GameObject m in menus)
        {
            m.SetActive(m == menu);
        }
        currentMenuIndex = menus.IndexOf(menu);
    }

    public void GoHome()
    {
        ShowMenu(homeMenuIndex);
    }

    public void SetupInitialMenus()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }

        if (menus.Count > homeMenuIndex)
        {
            menus[homeMenuIndex].SetActive(true);
            currentMenuIndex = homeMenuIndex;
        }
    }
}
