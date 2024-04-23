using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour
{
    // Reference to your canvas
    public GameObject canvasToHide;

    // Method to hide the canvas
    public void HideTheCanvas()
    {
        canvasToHide.SetActive(false);
    }
}
