using TMPro;
using UnityEngine;

public class TextMeshProVirtualKeyboardInputSource : MonoBehaviour
{
    [SerializeField]
    private OVRVirtualKeyboard virtualKeyboard;

    [SerializeField]
    private TMP_InputField inputField;

    private OVRVirtualKeyboard ovrVirtualKeyboard; // Reference to the OVRVirtualKeyboard component

    void Start()
    {
        ovrVirtualKeyboard = virtualKeyboard.GetComponent<OVRVirtualKeyboard>();

        inputField.onSelect.AddListener(OnInputFieldSelect);
        // inputField.onDeselect.AddListener(OnInputFieldDeselect);
        inputField.onValueChanged.AddListener(OnInputFieldValueChange);
        
        ovrVirtualKeyboard.CommitTextEvent.AddListener(OnCommitText); // Using AddListener for UnityEvent
        ovrVirtualKeyboard.BackspaceEvent.AddListener(OnBackspace);
        ovrVirtualKeyboard.KeyboardHiddenEvent.AddListener(OnKeyboardHidden);
    }

    private void OnInputFieldValueChange(string arg0)
    {
        ovrVirtualKeyboard.ChangeTextContext(arg0);
    }

    private void OnInputFieldSelect(string arg0)
    {
        ovrVirtualKeyboard.ChangeTextContext(arg0);
        virtualKeyboard.gameObject.SetActive(true);
    }

    // private void OnInputFieldDeselect(string arg0)
    // {
    //     virtualKeyboard.gameObject.SetActive(false);
    // }

    private void OnKeyboardHidden()
    {
        inputField.DeactivateInputField();
    }

    private void OnCommitText(string arg0)
    {
        virtualKeyboard.gameObject.SetActive(false);
        inputField.DeactivateInputField();
        inputField.text += arg0;
        inputField.MoveTextEnd(false);
    }

    private void OnBackspace()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            inputField.MoveTextEnd(false);
        }
    }
}
