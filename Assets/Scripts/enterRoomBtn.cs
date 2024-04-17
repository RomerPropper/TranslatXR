using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class enterRoomBtn : MonoBehaviour
{
    public GameObject ui;
    public GameObject nextUi;
    public GetRoomCode roomcodeManager;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (roomcodeManager.GetInputText() != ""){
            nextUi.SetActive(true);
            ui.SetActive(false);
        }

    }
}
