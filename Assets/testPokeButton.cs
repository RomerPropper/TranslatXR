using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class testPokeButton : MonoBehaviour
{
    public GameObject ui;
    // Start is called before the first frame update
    void Start()
    {
         GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    private void OnButtonClick()
    {
        Debug.Log("clicked");
        ui.SetActive(false);
    }
}
