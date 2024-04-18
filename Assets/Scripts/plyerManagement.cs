using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Serialization;

namespace Normal.Realtime
{
    [RequireComponent(typeof(Realtime))]
    public class plyerManagement : MonoBehaviour
    {
#pragma warning disable 0649 // Disable variable is never assigned to warning.
        [FormerlySerializedAs("_avatarPrefab")]
        [SerializeField] private GameObject _localAvatarPrefab;

        [SerializeField] private player.LocalPlayer _localPlayer;
#pragma warning restore 0649

        public GameObject localAvatarPrefab
        {
            get => _localAvatarPrefab;
            set => SetLocalAvatarPrefab(value);
        }

        public player localAvatar { get; private set; }
        public Dictionary<int, player> avatars { get; private set; }

        public delegate void AvatarCreatedDestroyed(plyerManagement avatarManager, player avatar, bool isLocalAvatar);
        public event AvatarCreatedDestroyed avatarCreated;
        public event AvatarCreatedDestroyed avatarDestroyed;
        private Dictionary<int, Transform> head = new Dictionary<int, Transform>();

        private Realtime _realtime;

        void Awake()
        {
            _realtime = GetComponent<Realtime>();
            _realtime.didConnectToRoom += DidConnectToRoom;

            if (_localPlayer == null)
                _localPlayer = new player.LocalPlayer();

            avatars = new Dictionary<int, player>();
        }

        private void OnEnable()
        {
            // Create avatar if we're already connected
            if (_realtime.connected)
                CreateAvatarIfNeeded();
        }

        private void OnDisable()
        {
            // Destroy avatar if needed
            DestroyAvatarIfNeeded();
        }

        void OnDestroy()
        {
            _realtime.didConnectToRoom -= DidConnectToRoom;
        }

        void DidConnectToRoom(Realtime room)
        {
            if (!gameObject.activeInHierarchy || !enabled)
                return;

            // Create avatar
            CreateAvatarIfNeeded();
        }

        public static player.DeviceType GetRealtimeAvatarDeviceTypeForLocalPlayer()
        {
            switch (XRSettings.loadedDeviceName)
            {
                case "OpenVR":
                    return player.DeviceType.OpenVR;
                case "Oculus":
                    return player.DeviceType.Oculus;
                default:
                    return player.DeviceType.Unknown;
            }
        }

        public void _RegisterAvatar(int clientID, player avatar)
        {
            if (avatars.ContainsKey(clientID))
            {
                Debug.LogError("RealtimeAvatar registered more than once for the same clientID (" + clientID + "). This is a bug!");
            }
            avatars[clientID] = avatar;
            if (avatar.head != null)
            {
                head[clientID] = avatar.head;
            }

            // Fire event
            if (avatarCreated != null)
            {
                try
                {
                    avatarCreated(this, avatar, clientID == _realtime.clientID);
                }
                catch (System.Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        public void _UnregisterAvatar(player avatar)
        {
            bool isLocalAvatar = false;

            List<KeyValuePair<int, player>> matchingAvatars = avatars.Where(keyValuePair => keyValuePair.Value == avatar).ToList();
            foreach (KeyValuePair<int, player> matchingAvatar in matchingAvatars)
            {
                int avatarClientID = matchingAvatar.Key;

                avatars.Remove(avatarClientID);

                isLocalAvatar = isLocalAvatar || avatarClientID == _realtime.clientID;
            }

            // Fire event
            if (avatarDestroyed != null)
            {
                try
                {
                    avatarDestroyed(this, avatar, isLocalAvatar);
                }
                catch (System.Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            int key = avatars.FirstOrDefault(x => x.Value == avatar).Key;
            if (key != 0)
            {
                avatars.Remove(key);
                head.Remove(key);
            }
        }

        private void SetLocalAvatarPrefab(GameObject localAvatarPrefab)
        {
            if (localAvatarPrefab == _localAvatarPrefab)
                return;

            _localAvatarPrefab = localAvatarPrefab;

            // Replace the existing avatar if we've already instantiated the old prefab.
            if (localAvatar != null)
            {
                DestroyAvatarIfNeeded();
                CreateAvatarIfNeeded();
            }
        }

        public void CreateAvatarIfNeeded()
        {
            if (!_realtime.connected)
            {
                Debug.LogError("RealtimeAvatarManager: Unable to create avatar. Realtime is not connected to a room.");
                return;
            }

            if (localAvatar != null)
                return;

            if (_localAvatarPrefab == null)
            {
                Debug.LogWarning("Realtime Avatars local avatar prefab is null. No avatar prefab will be instantiated for the local player.");
                return;
            }

            GameObject avatarGameObject = Realtime.Instantiate(_localAvatarPrefab.name, new Realtime.InstantiateOptions
            {
                ownedByClient = true,
                preventOwnershipTakeover = true,
                destroyWhenOwnerLeaves = true,
                destroyWhenLastClientLeaves = true,
                useInstance = _realtime,
            });

            if (avatarGameObject == null)
            {
                Debug.LogError("RealtimeAvatarManager: Failed to instantiate RealtimeAvatar prefab for the local player.");
                return;
            }

            localAvatar = avatarGameObject.GetComponent<player>();
            if (localAvatar == null)
            {
                Debug.LogError("RealtimeAvatarManager: Successfully instantiated avatar prefab, but could not find the RealtimeAvatar component.");
                return;
            }

            localAvatar.localPlayer = _localPlayer;
            localAvatar.deviceType = GetRealtimeAvatarDeviceTypeForLocalPlayer();
#if !UNITY_2020_2_OR_NEWER
#pragma warning disable 0618
            // Unity deprecated this API in 2020.2 without a clear replacement.
            localAvatar.deviceModel = XRDevice.model;
#pragma warning restore 0618
#endif
            Debug.Log("Position: " + localAvatar.transform.position);
        }

        public void DestroyAvatarIfNeeded()
        {
            if (localAvatar == null)
                return;

            Realtime.Destroy(localAvatar.gameObject);

            localAvatar = null;
        }

        public Dictionary<int, Vector3> GetAllheadPositions()
        {
            Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();
            foreach (var kvp in head)
            {
                positions[kvp.Key] = kvp.Value.position;
            }
            return positions;
        }

    }
}