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

    //This will notify Observers of the new message
    private void NotifyObservers() {
        Message newMessage = Message.parseFromJson(model.jsonMessage);
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
        NormcoreGM normcoreGMInstance = new NormcoreGM(); // Create an instance of NormcoreGM
        ProfileClass profile = normcoreGMInstance.GetProfile(); // Retrieve the profile

        Message newMessage = Message.parseFromJson(model.jsonMessage);
        newMessage.MessageContent = await Translator.Translate(newMessage.MessageContent, newMessage.Language, profile.Language); //Translate new message
        model.jsonMessage = newMessage.convertToJson(); //Update model with translated message

        profile.Messages.Add(Message.parseFromJson(model.jsonMessage));
        NotifyObservers();
    }

    //Use this function anytime you want to send a message across the network.
    //This will update the model with the message details which will sync to all other clients.
    //This needs to be called from the game master after a transcription and sentiment analytsis has been done.
    public void SendMessage(Message newMessage) {
        model.jsonMessage = newMessage.convertToJson();
    }

}
