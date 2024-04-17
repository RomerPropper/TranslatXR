using UnityEngine;
using TMPro;

public class HandInputFieldSelector : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to the TextMeshPro Input Field

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is a hand
        if (collision.gameObject.CompareTag("Hand"))
        {
            // Select the input field
            inputField.Select();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the colliding object is a hand
        if (collision.gameObject.CompareTag("Hand"))
        {
            // Deselect the input field
            inputField.DeactivateInputField();
        }
    }
}
