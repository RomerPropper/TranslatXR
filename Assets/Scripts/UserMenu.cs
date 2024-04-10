using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMenu : MonoBehaviour
{
    [SerializeField] ChatController chatController;
    [SerializeField] GameObject menuRoot;
    [SerializeField] Transform startPage;
    public void OpenStartPage()
    {
        chatController.OpenPage(startPage);
    }
    public void ToggleMenu(bool isOn)
    {
        menuRoot.SetActive(isOn);
    }
}
