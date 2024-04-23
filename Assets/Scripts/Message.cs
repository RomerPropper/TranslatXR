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
    public string Username { get; set; }
    public string MessageContent { get; set; }
    public DateTime Timestamp { get; set; }
    public string SentimentAnalysis { get; set; }
    public string Language { get; set; }
    public int ClientID { get; set; }

    public Message() { }

    public Message(string username, string message, string lang, string sentimentAnalysis, int clientID)
    {
        this.Username = username;
        this.MessageContent = message;
        this.Timestamp = DateTime.Now;
        this.SentimentAnalysis = sentimentAnalysis;
        this.Language = lang;
        this.ClientID = clientID;
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
