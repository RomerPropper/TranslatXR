using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class langselectbtn : MonoBehaviour
{
    public GameObject langselectUI;
    //public NormcoreGM normcoreGM;
    public Button continueButton;
    public TMP_Dropdown langDropdown;
    public TMP_InputField nameInput;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = continueButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick(){
        string name = nameInput.text;
        string lang = langDropdown.options[langDropdown.value].text;
        if (name == ""){
            Debug.Log("enter a name");
            return;
        }
        else if(lang=="English"){
            //normcoreGM.SetLangEnglish();
        }
        else if(lang=="Chinese"){
            //normCoreGM.SetLangChinese();
        }
        else{
            Debug.Log("no language");
        }
        Debug.Log(lang);
        langselectUI.SetActive(false);
    }
}
