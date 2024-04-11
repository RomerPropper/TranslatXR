using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;


public class ChatBubbleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject chatBubblePrefab;

    [SerializeField]
    private Realtime _realtime;

    private Dictionary<RealtimeAvatar, GameObject> chatBubbles = new Dictionary<RealtimeAvatar, GameObject>();

    private void Start()
    {
        // Get the RealtimeAvatarManager from the scene
        RealtimeAvatarManager avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        if (avatarManager != null)
        {
            avatarManager.avatarCreated += OnAvatarCreated;
            avatarManager.avatarDestroyed += OnAvatarDestroyed;
        }
        else
        {
            // Debug.LogError("ChatBubbleManager: Failed to find RealtimeAvatarManager in the scene.");
        }
    }


    private void OnAvatarCreated(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if (!isLocalAvatar && !chatBubbles.ContainsKey(avatar)) // Assuming you don't want to create a chat bubble for the local user's avatar
        {
            CreateChatBubbleForAvatar(avatar);
        }
    }

    void CreateChatBubbleForAvatar(RealtimeAvatar avatar)
    {
        GameObject chatBubble = Instantiate(chatBubblePrefab);
        // Assuming each avatar has a child with a specific name that represents the camera or head. Adjust as necessary.
        Transform cameraTransform = avatar.GetComponentInChildren<Camera>()?.transform; // This assumes the avatar prefab contains a Camera component representing the user's viewpoint.
        
        if(cameraTransform != null)
        {
            ChatBubblePositioner positioner = chatBubble.GetComponent<ChatBubblePositioner>();
            if(positioner != null)
            {
                positioner.SetTarget(cameraTransform);
                chatBubbles.Add(avatar, chatBubble); // Add to your dictionary to keep track
            }
        }
        else
        {
            Destroy(chatBubble); // Cleanup if we can't find the correct transform
        }
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions
        RealtimeAvatarManager avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        if (avatarManager != null)
        {
            avatarManager.avatarCreated -= OnAvatarCreated;
            avatarManager.avatarDestroyed -= OnAvatarDestroyed;
        }
    }

    private void OnAvatarDestroyed(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if (chatBubbles.TryGetValue(avatar, out GameObject chatBubble))
        {
            Destroy(chatBubble);
            chatBubbles.Remove(avatar);
        }
    }

}
