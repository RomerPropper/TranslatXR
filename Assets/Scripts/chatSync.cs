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
        if (model.lang != normcoreGM.getTargetLang()) {
            model.chatText = await TranslateBeforeChange(model.chatText);
        }
        UpdateChatText();
    }

    private async Task<string> TranslateBeforeChange(string newText) {
        string myLang = normcoreGM.getTargetLang();
        string[] lines = newText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string lastLine = lines.LastOrDefault();
        string translatedText = await Translator.Translate(lastLine, model.lang, myLang);
        return newText += translatedText;
    }


    //This method should really not be used. Mostly here for initlization.
    public void SetText(string message, string lang) {
        SetLanguage(lang);
        model.chatText = message;
    }

    //This is the main method that will be called when we want to add text to the chatbox.
    public void AddText(string message, string lang) {
        SetLanguage(lang);
        model.chatText += message + "\n";
    }

    public void SetLanguage(string lang) {
        model.lang = lang;
    }

    private string GetLanguage() {
        return model.lang;
    }

}
