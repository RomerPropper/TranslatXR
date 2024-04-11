using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour
{
    [SerializeField] PageController pageController;
    [SerializeField] GameObject menuRoot;
    [SerializeField] Transform startPage;
    public void OpenStartPage()
    {
        pageController.OpenPage(startPage);
    }
    public void ToggleMenu(bool isOn)
    {
        menuRoot.SetActive(isOn);
    }
}
