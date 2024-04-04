using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NormcoreGM : MonoBehaviour
{
    public TMP_InputField inputField_username;
    public TMP_Dropdown inputField_ln;

    public ProfileClass profile = new ProfileClass();

    [SerializeField] private string _chatText = default;
    public chatSync _chatSync;
    private string _targetLang = "en";

    public TextMeshProUGUI statusText;
    public int recordLength = 5;
    private bool _isRecording = false;
    private bool _isReadyForNextRecording = true;
    private AudioClip recordedClip;
    
    private float[] _audioSampleBuffer = new float[1024];
    private int _sampleRate = 44100;
    private const float VolumeThreshold = 0.09f; 

    private AudioClip monitoringClip; 
    private float timeSinceLastSpeech = 0f;
    private const float speechCooldown = 3f;

    void Start()
    {
        _sampleRate = AudioSettings.outputSampleRate;
        monitoringClip = Microphone.Start(null, true, 10, _sampleRate);
        while (!(Microphone.GetPosition(null) > 0)) {} 
        UpdateStatusText();
    }

    void Update()
    {
        if (!_isRecording && _isReadyForNextRecording)
        {
            if (IsSpeaking())
            {
                Debug.Log("Detected speech, starting recording.");
                StartRecording();
            }
        }
        else if (_isRecording)
        {
            if (!IsSpeaking())
            {
                timeSinceLastSpeech += Time.deltaTime;
                Debug.Log($"Not speaking. Cooldown: {timeSinceLastSpeech}");

                if (timeSinceLastSpeech >= speechCooldown)
                {
                    Debug.Log("Cooldown elapsed, stopping recording.");
                    StopRecording();
                    _Transcribe();
                }
            }
            else
            {
                timeSinceLastSpeech = 0f;
            }
        }
    }


    private bool IsSpeaking()
    {
        float maxVolume = 0f;
        int micPosition = Microphone.GetPosition(null) - (_audioSampleBuffer.Length + 1);

        if (micPosition < 0) return false;

        monitoringClip.GetData(_audioSampleBuffer, micPosition);
        
        foreach (var sample in _audioSampleBuffer)
        {
            maxVolume = Mathf.Max(maxVolume, Mathf.Abs(sample));
        }
        Debug.Log($"IsSpeaking: Max Volume = {maxVolume}");

        return maxVolume > VolumeThreshold;
    }

    // =============== RECORDING ===============
    private void ReadyForNextRecording()
    {
        _isReadyForNextRecording = true;
    }

    private void StartRecording()
    {
        if (_isRecording) return;

        _isRecording = true;
        recordedClip = Microphone.Start(null, false, recordLength, _sampleRate);
        Debug.Log("Recording Started");

        UpdateStatusText();
    }

    private void StopRecording()
    {
        if (!_isRecording) return;

        _isRecording = false;
        Microphone.End(null); 
        Debug.Log("Recording Ended");

        UpdateStatusText();

        _isReadyForNextRecording = false; 
        Invoke("ResetRecordingState", 1f); 
    }

    private void ResetRecordingState()
    {
        monitoringClip = Microphone.Start(null, true, 10, _sampleRate);
        while (!(Microphone.GetPosition(null) > 0)) {} 
        _isReadyForNextRecording = true;
        timeSinceLastSpeech = 0f;
        Debug.Log("Ready for next recording");
    }
    // =============== RECORDING ===============

    private void UpdateStatusText()
    {
        statusText.text = _isRecording ? "Recording..." : "Not Recording";
    }

    private async void _Transcribe()
    {
        string transcription = await Translator.Transcribe(recordedClip, _targetLang);
        postTranscription(transcription);
    }

    public void postTranscription(string message) {
        Message newMessage = new Message(profile.UserName, message, profile.Language, "Unknown");
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

    public void GetInputLanguage()
    {
        string lang = inputField_ln.options[inputField_ln.value].text;
        profile.Language = inputField_ln.options[inputField_ln.value].text;
        Debug.Log(profile.Language);
        if (lang == "English"){
            SetLangEnglish();
        }
        else if (lang == "Spanish"){
            SetLangChinese();
        }
    }

    public void GetInputUserName()
    {
        profile.UserName = inputField_username.text;
        Debug.Log(profile.UserName);
    }

    public void joinAnnouncement()
    {
        string Announcement = profile.Language + " speaker " + profile.UserName + " has joined the room!";
        Message newMessage = new Message(profile.UserName, Announcement, profile.Language, "Unknown");
        Debug.Log(Announcement);
        _chatSync.SendMessage(newMessage);
    }

    public ProfileClass GetProfile() { return profile; }

}
