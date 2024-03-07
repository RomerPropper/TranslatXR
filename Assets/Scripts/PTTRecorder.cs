using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PTTRecorder : MonoBehaviour
{
    
    public OVRInput.Button pttButton = OVRInput.Button.PrimaryIndexTrigger;
    public int recordLength = 5;
    public TextMeshProUGUI statusText;
    public NormcoreGM normcoreGM;

    private bool _isRecording = false;
    private AudioClip recordedClip;

    void Start()
    {
        if (!_isRecording)
        {
            statusText.text = "Not Recording";
        }
        else {
            statusText.text = "Recording...";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(pttButton))
        {
            
            _isRecording = true;
            statusText.text = "Recording...";
            StartRecording();
        }
        else if (OVRInput.GetUp(pttButton) && _isRecording) {
            _isRecording= false;
            statusText.text = "Not Recording";
            StopRecording();
        }
    }

    private void StartRecording() {
        recordedClip = Microphone.Start(null, false, recordLength, 44100);
    }

    private static void StopRecording()
    {
        Microphone.End(null);
    }
}
