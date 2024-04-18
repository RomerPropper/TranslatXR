using TMPro;
using UnityEngine;

public class TextMeshProVirtualKeyboardInputSource : MonoBehaviour
{
    [SerializeField]
    private OVRVirtualKeyboard virtualKeyboard;

    [SerializeField]
    private TMP_InputField inputField;

    void Start()
    {
        inputField.onSelect.AddListener(OnInputFieldSelect);
        // inputField.onDeselect.AddListener(OnInputFieldDeselect);
        inputField.onValueChanged.AddListener(OnInputFieldValueChange);

        virtualKeyboard.CommitTextEvent.AddListener(OnCommitText);
        virtualKeyboard.BackspaceEvent.AddListener(OnBackspace);
        virtualKeyboard.KeyboardHiddenEvent.AddListener(OnKeyboardHidden);
    }

    private void OnInputFieldValueChange(string arg0)
    {
        virtualKeyboard.ChangeTextContext(arg0);
    }

    private void OnInputFieldSelect(string arg0)
    {
        virtualKeyboard.ChangeTextContext(arg0);
        virtualKeyboard.gameObject.SetActive(true);
    }

    private void OnInputFieldDeselect(string arg0)
    {
        virtualKeyboard.gameObject.SetActive(false);
    }

    private void OnKeyboardHidden()
    {
        inputField.DeactivateInputField();
    }

    private void OnCommitText(string arg0)
{
    if (arg0 == "\n") {
        virtualKeyboard.gameObject.SetActive(false);
        inputField.DeactivateInputField();
    } else {
        inputField.text += arg0;
        inputField.MoveTextEnd(false);
    }
}

    private void OnBackspace()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            inputField.MoveTextEnd(false);
        }
    }

    private void RemoveListeners()
    {
        inputField.onSelect.RemoveListener(OnInputFieldSelect);
        inputField.onValueChanged.RemoveListener(OnInputFieldValueChange);
        // inputField.onDeselect.RemoveListener(OnInputFieldDeselect);
    }

    public void HideKeyboard()
    {
        inputField.text = ""; // Clear text
        inputField.DeactivateInputField(); // Deselect field
        virtualKeyboard.gameObject.SetActive(false);
        RemoveListeners();
    }
}
