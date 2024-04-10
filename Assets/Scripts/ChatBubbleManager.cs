using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class ChatBubbleManager : MonoBehaviour
{
    public GameObject chatBubblePrefab; 
    public Realtime realtime; 

    // Call this method when you need to create a chat bubble for a user
    private void Awake()
    {
        // Find the Realtime component in the scene
        realtime = FindObjectOfType<Realtime>();
    }
    public void CreateChatBubbleForUser(RealtimeAvatar userAvatar)
    {
        GameObject chatBubble = Instantiate(chatBubblePrefab, userAvatar.head.position, Quaternion.identity);
        chatBubble.GetComponent<ChatBubble>().SetTarget(userAvatar.head);
    }

    public void TryCreateChatBubbleForUser(RealtimeAvatar userAvatar)
    {
        // Check if we're connected to a room before creating a chat bubble
        if (realtime.connected)
        {
            GameObject chatBubble = Instantiate(chatBubblePrefab, userAvatar.head.position, Quaternion.identity);
            chatBubble.GetComponent<ChatBubble>().SetTarget(userAvatar.head);
        }
    }
}
