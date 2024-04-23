using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FontManager : MonoBehaviour
{
    public TextMeshProUGUI tmp_font;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
        tmp_font.fontSize = 50;
        
    }

    private void OnButtonClick()
    {
        tmp_font.fontSize += 1;
    }
}
