using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class texttest : MonoBehaviour
{
    [SerializeField]
    private string _inputText = default;
    private string _previoustext = default;

    private Text _Text;

    private void Awake()
    {
        // Get a reference to the color sync component
        _Text = GetComponent<Text>();
    }

    private void Update()
    {
        // If the color has changed (via the inspector), call SetColor on the color sync component.
        if (_inputText != _previoustext)
        {
            _Text.SetText(_inputText);
            _previoustext = _inputText;
        }
    }
}
