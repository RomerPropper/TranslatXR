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

    public delegate void MessageChangedEventHandler(Message newMessage);
    public event MessageChangedEventHandler MessageChanged;

    //This will notify Observers of the new message
    private void NotifyObservers(Message newMessage) {
        MessageChanged?.Invoke(newMessage);
    }

    protected override void OnRealtimeModelReplaced(chatSyncModel previousModel, chatSyncModel currentModel)
    {
        if (previousModel != null) {
            previousModel.jsonMessageDidChange -= JsonMessageDidChange;
        }

        if (currentModel != null) {
            NotifyObservers(Message.parseFromJson(currentModel.jsonMessage));

            currentModel.jsonMessageDidChange += JsonMessageDidChange;
        }
    }

    private void JsonMessageDidChange(chatSyncModel model, string value)
    {
        NotifyObservers(Message.parseFromJson(model.jsonMessage));
    }

    //Use this function anytime you want to 
    public void SendMessage(Message newMessage) {
        model.jsonMessage = newMessage.convertToJson();
    }

}
