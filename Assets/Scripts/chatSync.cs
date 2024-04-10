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
    //This will allow classes to "subscribe" to any new Messages so they are notified.
    public delegate void MessageChangedEventHandler(Message newMessage);
    public event MessageChangedEventHandler MessageChanged;

    public NormcoreGM normcoreGM;

    public TextMeshProUGUI testText;

    //This will notify Observers of the new message
    private void NotifyObservers() {
        Debug.Log("Recieved: " + model.jsonMessage);
        Message newMessage = Message.parseFromJson(model.jsonMessage);
        testText.text = newMessage.MessageContent;
        MessageChanged?.Invoke(newMessage);
    }

    protected override void OnRealtimeModelReplaced(chatSyncModel previousModel, chatSyncModel currentModel)
    {
        if (previousModel != null) {
            previousModel.jsonMessageDidChange -= JsonMessageDidChange;
        }

        if (currentModel != null) {
            NotifyObservers();

            currentModel.jsonMessageDidChange += JsonMessageDidChange;
        }
    }

    private async void JsonMessageDidChange(chatSyncModel model, string value)
    {
        Debug.Log("Message Changed: " + model.jsonMessage);
        Message newMessage = Message.parseFromJson(model.jsonMessage);
        newMessage.MessageContent = await Translator.Translate(newMessage.MessageContent, newMessage.Language, this.normcoreGM.profile.Language); //Translate new message
        model.jsonMessage = newMessage.convertToJson(); //Update model with translated message

        if (this.normcoreGM is null)
        {
            Debug.Log("NORMCORE IS NULL");
        }
        else if (this.normcoreGM.profile is null) {
            Debug.Log("PROFILE IS NULL");
        }
        else if (this.normcoreGM.profile.Messages is null)
        {
            Debug.Log("MESSAGES ARE NULL");
        }
        //this.normcoreGM.profile.Messages.Add(Message.parseFromJson(model.jsonMessage));
        NotifyObservers();
    }

    //Use this function anytime you want to send a message across the network.
    //This will update the model with the message details which will sync to all other clients.
    //This needs to be called from the game master after a transcription and sentiment analytsis has been done.
    public void SendMessage(Message newMessage) {
        Debug.Log("Sent: " + newMessage.convertToJson());
        model.jsonMessage = newMessage.convertToJson();
    }

}
