using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FontManager : MonoBehaviour
{
    public TextMeshProUGUI tmp_font;
    public int fontSize;
    // Start is called before the first frame update
    void Start()
    {
       // GetComponent<Button>().onClick.AddListener(OnButtonClick);
        tmp_font.fontSize = fontSize;
    }

    public void FontUp()
    {
        tmp_font.fontSize += 2;
    }
    public void FontDown()
    {
        tmp_font.fontSize -= 2;
    }
}
