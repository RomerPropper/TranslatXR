using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class ChatBubbleManager : MonoBehaviour
{
    public GameObject chatBubblePrefab; // Assign your chat bubble prefab here

    // Call this method when you need to create a chat bubble for a user
    public void CreateChatBubbleForUser(RealtimeAvatar userAvatar)
    {
        GameObject chatBubble = Instantiate(chatBubblePrefab, userAvatar.head.position, Quaternion.identity);
        chatBubble.GetComponent<ChatBubble>().SetTarget(userAvatar.head);
    }
}
