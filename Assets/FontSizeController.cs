using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontSizeController : MonoBehaviour
{
    public Text textElement;
    public Slider fontSizeSlider;
    public int fontSize;

    void Start()
    {
        // Initialize the slider value and text font size here if needed
        fontSizeSlider.value = fontSize;
    }

    public void OnSliderValueChanged()
    {
        fontSize = (int)fontSizeSlider.value;
    }
}

