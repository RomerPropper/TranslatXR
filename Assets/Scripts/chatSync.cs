using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class chatSync : RealtimeComponent<chatSyncModel>
{

    public TextMeshProUGUI textMeshProChatText;
    public NormcoreGM normcoreGM;

    private void UpdateChatText() {
        textMeshProChatText.text = model.chatText;
    }

    protected override void OnRealtimeModelReplaced(chatSyncModel previousModel, chatSyncModel currentModel)
    {
        if (previousModel != null) {
            previousModel.chatTextDidChange -= ChatTextDidChange;
        }

        if (currentModel != null) {
            if (currentModel.isFreshModel) {
                currentModel.chatText = textMeshProChatText.text;
            }

            UpdateChatText();

            currentModel.chatTextDidChange += ChatTextDidChange;
        }
    }

    private async void ChatTextDidChange(chatSyncModel model, string value) {
        
        string[] lines = model.chatText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string newLineAdded = lines.LastOrDefault();

        string newMessage = newLineAdded.Split(";")[0];
        string srcLang = newLineAdded.Split(";")[1];
        string myLang = normcoreGM.getTargetLang();
        if (srcLang != myLang)
        {
            string translatedNewMessage = await TranslateBeforeChange(newMessage, srcLang);
            lines[lines.Length - 1] = translatedNewMessage;
            model.chatText = string.Join(Environment.NewLine, lines);
        }
        else {
            lines[lines.Length - 1] = newMessage;
            model.chatText = string.Join(Environment.NewLine, lines);
        }
        UpdateChatText();
    }

    private async Task<string> TranslateBeforeChange(string newText, string srcLang) {
        string myLang = normcoreGM.getTargetLang();
        string translatedText =  await Translator.Translate(newText, srcLang, myLang);
        return translatedText;
    }


    //This method should really not be used. Mostly here for initlization.
    public void SetText(string message, string lang) {
        model.chatText = message + ";" + lang;
    }

    //This is the main method that will be called when we want to add text to the chatbox.
    public void AddText(string message, string lang) {
        model.chatText += message + ";" + lang + "\n";
    }

}
