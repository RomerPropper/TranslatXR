using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NormcoreGM : MonoBehaviour
{
    public TMP_InputField inputField_username;
    public TMP_Dropdown inputField_ln;

    public ProfileClass profile { get; set; }

    [SerializeField] private string _chatText = default;
    public chatSync _chatSync;

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
        this.profile = new ProfileClass();
        this.profile.Messages = new List<Message>();
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
                // Testing Only
                // Debug.Log($"Not speaking. Cooldown: {timeSinceLastSpeech}");

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
        // TESTING ONLY
        // Debug.Log($"IsSpeaking: Max Volume = {maxVolume}");

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
        Invoke("ResetRecordingState", 2f); 
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

    //Retrives the transcription of the audio file
    private async void _Transcribe()
    {
        string transcription = await Translator.Transcribe(recordedClip, this.profile.Language);
        postTranscription(transcription);
    }

    //Sends the transcription over normcore
    //TODO: Add sentiment analysis. Right now it is set to Unknown
    public void postTranscription(string message) {
        Message newMessage = new Message(profile.UserName, message, profile.Language, "Unknown");

        _chatSync.SendMessage(newMessage);
    }

    //Sets the profile language to english
    public void SetLangEnglish() {
        Debug.Log("Language set to English");
        this.profile.Language = "en";
    }

    //Sets the profile langauge to spanish
    public void SetLangSpanish()
    {
        Debug.Log("Language set to Spanish");
        this.profile.Language = "es";
    }

    //Gets the inputed language and sets it on the profile
    public void GetInputLanguage()
    {
        string lang = inputField_ln.options[inputField_ln.value].text;
        profile.Language = inputField_ln.options[inputField_ln.value].text;
        Debug.Log("Language Inputed: " + profile.Language);
        if (lang == "English"){
            SetLangEnglish();
        }
        else if (lang == "Spanish"){
            SetLangSpanish();
        }
    }

    //Gets the inputed username
    public void GetInputUserName()
    {
        profile.UserName = inputField_username.text;
        Debug.Log("Name Inputed: " + profile.UserName);
    }

    public void joinAnnouncement()
    {
        string Announcement = profile.Language + " speaker " + profile.UserName + " has joined the room!";
        Message newMessage = new Message(profile.UserName, Announcement, profile.Language, "Unknown");
        Debug.Log(Announcement);
        _chatSync.SendMessage(newMessage);
    }

}
