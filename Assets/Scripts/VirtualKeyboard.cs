using UnityEngine;
using TMPro;

public class TMPInputFieldFocusDetector : MonoBehaviour
{
    public OVRVirtualKeyboard keyboard; // Reference to the OVRVirtualKeyboard
    public TMP_InputField inputField; // Reference to the TextMeshPro Input Field

    void Start()
    {
        // Subscribe to the events
        inputField.onSelect.AddListener(OnInputFieldSelect);
        inputField.onDeselect.AddListener(OnInputFieldDeselect);
    }

    // Called when the input field is selected (focused)
    void OnInputFieldSelect()
    {
        ShowVirtualKeyboard();
    }

    // Called when the input field is deselected (loses focus)
    void OnInputFieldDeselect()
    {
        HideVirtualKeyboard();
    }

    // Show virtual keyboard
    void ShowVirtualKeyboard()
    {
        if (keyboard != null)
        {
            keyboard.Show();
        }
    }

    // Hide virtual keyboard
    void HideVirtualKeyboard()
    {
        if (keyboard != null)
        {
            keyboard.Hide();
        }
    }
}
