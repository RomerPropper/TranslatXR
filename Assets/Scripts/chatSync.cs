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

    //This will notify Observers of the new message
    private void NotifyObservers(Message newMessage) {
        Debug.Log("Recieved: " + newMessage.convertToJson());
        //Message newMessage = Message.parseFromJson(model.jsonMessage);
        MessageChanged?.Invoke(newMessage);
    }

    protected override void OnRealtimeModelReplaced(chatSyncModel previousModel, chatSyncModel currentModel)
    {
        if (previousModel != null) {
            previousModel.jsonMessageDidChange -= JsonMessageDidChange;
        }

        if (currentModel != null) {
            currentModel.jsonMessageDidChange += JsonMessageDidChange;
        }
    }

    private async void JsonMessageDidChange(chatSyncModel model, string value)
    {
        //Error checking
        if (this.normcoreGM is null) {
            Debug.LogError("chatSync [JsonMessageDidChange]: normcoreGM is not set to a reference.");
        }
        if (this.normcoreGM.playerManager is null)
        {
            Debug.LogError("chatSync [JsonMessageDidChange]: normcoreGM.playerManager is not set to a reference.");
        }
        //If our language is not set, then we must not be past the profile screen and we will not react to any messages
        if (this.normcoreGM.profile.Language == null || this.normcoreGM.profile.Language == "") {
            return;
        }

        Message newMessage = Message.parseFromJson(model.jsonMessage);

        //Ignore message if we are the sender, the message is blank or null, or if the clientid is does not exists
        if (newMessage.ClientID == this.normcoreGM.playerManager.localAvatar.realtimeView.ownerIDSelf || newMessage.MessageContent == "" || newMessage.MessageContent == null || newMessage.ClientID == -1 || newMessage.ClientID == null) {
            Debug.Log("Ignoring...");
            return;
        }
        Debug.Log("Message Changed: " + model.jsonMessage);
        //Translate only if their language is different from ours
        if (newMessage.Language.ToLower() != this.normcoreGM.profile.Language.ToLower()) {
            newMessage.MessageContent = await Translator.Translate(newMessage.MessageContent, newMessage.Language, this.normcoreGM.profile.Language); //Translate new message
        }
        //model.jsonMessage = newMessage.convertToJson(); //Update model with translated message
        NotifyObservers(newMessage);
    }

    //Use this function anytime you want to send a message across the network.
    //This will update the model with the message details which will sync to all other clients.
    //This needs to be called from the game master after a transcription and sentiment analytsis has been done.
    public void SendMessage(Message newMessage) {
        Debug.Log("Sent: " + newMessage.convertToJson());
        model.jsonMessage = newMessage.convertToJson();
    }

}
