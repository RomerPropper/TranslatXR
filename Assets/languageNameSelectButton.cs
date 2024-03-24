using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class languageNameSelectButton : MonoBehaviour
{
    public GameObject langselectUI;
    public NormcoreGM normcoreGM;
    public Button continueButton;
    public TMP_Dropdown langDropdown;
    public TMP_InputField nameInput;

    void Start()
    {
        Button btn = continueButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void TaskOnClick(){
        string name = nameInput.text;
        string lang = langDropdown.options[langDropdown.value].text;
        if (name == ""){
            return;
        }
        if ( lang == "English"){
            normcoreGM.SetLangEnglish();
        }
        else{
            normcoreGM.SetLangChinese();
        }
        Debug.Log(lang);
        langselectUI.SetActive(false);

    
    }
}
