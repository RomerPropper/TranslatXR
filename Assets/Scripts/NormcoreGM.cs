using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NormcoreGM : MonoBehaviour
{


    //Normcore variables
    [SerializeField]
    private string _chatText = default;

    public chatSync _chatSync;
    private string _targetLang = "en";


    //PTT Variables
    public OVRInput.Button pttButton = OVRInput.Button.PrimaryIndexTrigger;
    public KeyCode alternatePTTButton = KeyCode.R;
    public int recordLength = 5;
    public TextMeshProUGUI statusText;

    private bool _isRecording = false;
    private AudioClip recordedClip;

    void Start()
    {
        if (!_isRecording)
        {
            statusText.text = "Not Recording";
        }
        else
        {
            statusText.text = "Recording...";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(pttButton) || Input.GetKeyDown(alternatePTTButton))
        {
            if (!_isRecording && _targetLang != null) {
                _isRecording = true;
                statusText.text = "Recording...";
                StartRecording();
            }
        }
        else if (OVRInput.GetUp(pttButton) || Input.GetKeyUp(alternatePTTButton))
        {
            if (_isRecording && _targetLang != null) {
                _isRecording = false;
                statusText.text = "Not Recording";
                StopRecording();
                _Transcribe();
            }
        }
    }

    private async void _Transcribe() {
        string tanscription = await Translator.Transcribe(recordedClip, _targetLang);
        postTranscription(tanscription);
    }

    private void StartRecording()
    {
        recordedClip = Microphone.Start(null, false, recordLength, 44100);
    }

    private static void StopRecording()
    {
        Microphone.End(null);
    }

    public void postTranscription(string message) {
        Message newMessage = new Message("Unknown", message, _targetLang, "Unknown");
        _chatSync.SendMessage(newMessage);
    }

    public void SetLangEnglish() {
        Debug.Log("Language set to English");
        _targetLang = "en";
    }

    public void SetLangChinese()
    {
        Debug.Log("Language set to Chinese");
        _targetLang = "es";
    }

    public string getTargetLang() {
        return _targetLang;
    }
}
