using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class PlayerManager : MonoBehaviour
{
    private RealtimeAvatarManager avatarManager;
    public ChatBubbleController localChatBubbleController; // Assign this via the inspector or find it at runtime

    private void Start()
    {
        avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        avatarManager.avatarCreated += AvatarCreated;
        // You could find the local ChatBubbleController component here if not assigned
        localChatBubbleController = FindObjectOfType<ChatBubbleController>(); 
    }

    private void AvatarCreated(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if (!isLocalAvatar)
        {
            Transform otherPlayerHeadAnchor = FindHeadAnchor(avatar);
            localChatBubbleController.SetupOtherPlayerHeadAnchor(otherPlayerHeadAnchor);
            localChatBubbleController.ShowMyChatBubble();
        }
    }

    private Transform FindHeadAnchor(RealtimeAvatar avatar)
    {
        // Assuming the HeadAnchor is a named child of the avatar GameObject
        return avatar.transform.Find("HeadAnchor");
    }

    private void OnDestroy()
    {
        if (avatarManager != null)
        {
            avatarManager.avatarCreated -= AvatarCreated;
        }
    }
}

