// TODO: localize (i.e. dynamically choose credits_{lang} depending on user's chosen language)

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDocAdder : MonoBehaviour
{
    private TextMeshPro _text;
    public TextAsset _document;
    private void Awake()
    {
        _text = this.gameObject.GetComponent<TextMeshPro>();
        _text.text = _document.text;
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
