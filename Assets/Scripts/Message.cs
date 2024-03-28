using Meta.WitAi.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This is the main class that will store information about a message.
username = the senders username
message = The message before translation
timestamp =  the time the message was sent
sentimentAnalysis = the analysis of the persons sentiment
*/
public class Message
{
    private string username;
    private string message;
    private DateTime timestamp;
    private string sentimentAnalysis;
    Message(string username, string message, DateTime timestamp, string sentimentAnalysis) {
        this.username = username;
        this.message = message;
        this.timestamp = timestamp;
        this.sentimentAnalysis = sentimentAnalysis;
    }

    //Parses the json string into Message class
    public static Message parseFromJson(string json) {
        return JsonConvert.DeserializeObject<Message>(json);
    }

    //Converts this class into a json string
    public string convertToJson() {
        return JsonConvert.SerializeObject(this);
    }
}
