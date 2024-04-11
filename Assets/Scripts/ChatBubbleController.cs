using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;


public class ChatBubbleController : MonoBehaviour
{
    public GameObject chatBubblePrefab; 
    private GameObject instantiatedChatBubble;

    public Transform myHeadAnchor; 
    private Transform otherPlayerHeadAnchor;

    public void SetupOtherPlayerHeadAnchor(Transform otherHeadAnchor)
    {
        otherPlayerHeadAnchor = otherHeadAnchor;
        if (instantiatedChatBubble == null)
        {
            instantiatedChatBubble = Instantiate(chatBubblePrefab);
        }
        instantiatedChatBubble.transform.SetParent(otherPlayerHeadAnchor, false);
        instantiatedChatBubble.transform.localPosition = Vector3.up;
    }

    public void ShowMyChatBubble()
    {
        if (instantiatedChatBubble != null)
        {
            instantiatedChatBubble.SetActive(true);
        }
    }

    public void HideMyChatBubble()
    {
        if (instantiatedChatBubble != null)
        {
            instantiatedChatBubble.SetActive(false);
        }
    }

    public void RemoveChatBubble()
    {
        if (instantiatedChatBubble != null)
        {
            Destroy(instantiatedChatBubble);
        }
    }
}