
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.UdonPortal
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UdonPortal : UdonPortalBase
    {
        [SerializeField] private GameObject portalMarkerPrefab;
        [SerializeField] private MessageManager messageManager;
        private GameObject previousPortal;
        private string previousRoomId;
        public bool canEnter;
        public void AllowEnter()
        {
            canEnter = true;
        }

        private void Start()
        {
            CheckPortalRoomId(); //ループスタート
        }

        /// <summary>
        /// Room IDから新しいポータルを作成します。
        /// </summary>
        public void NewPortal(string roomId)
        {
            GenerateNewPortalAll(roomId);
        }

        /// <summary>
        /// IDやリージョンなどを指定して新しいポータルを作成します。
        /// Publicインスタンスを作成します。
        /// </summary>
        public void NewPortal(string worldId, string instanceId, Region region)
        {
            GenerateNewPortalAll($"{FString("", worldId)}{FString(":", instanceId)}~{GetRegion(region)}");
        }

        /// <summary>
        /// IDやリージョンなどを指定して新しいポータルを作成します。
        /// Friend+からInviteインスタンスを作成します。
        /// </summary>
        public void NewPortal(string worldId, string instanceId, string userId, InstanceType instanceType, Region region)
        {
            GenerateNewPortalAll($"{FString("", worldId)}{FString(":", instanceId)}{GetInstanceTypeString(instanceType, userId)}~{GetRegion(region)}");
        }

        /// <summary>
        /// IDやリージョンなどを指定して新しいポータルを作成します。
        /// グループのインスタンスタイプとIDを指定します。
        /// </summary>
        public void NewPortal(string worldId, string instanceId, string groupId, GroupType groupType, Region region)
        {
            GenerateNewPortalAll($"{FString("", worldId)}{FString(":", instanceId)}{GetGroupTypeString(groupType, groupId)}~{GetRegion(region)}");
        }

        private void GenerateNewPortalAll(string id)
        {
            SyncRoomId(id);
            GenerateNewPortal(id);
            Message("Generated!");
        }

        private void GenerateNewPortal(string id)
        {
            //GameObject名前空間は省略できないので注意
            if (Utilities.IsValid(previousPortal))
            {
                GameObject.Destroy(previousPortal);
            }
            VRCPortalMarker portal = portalMarkerPrefab.GetComponent<VRCPortalMarker>();
            portal.roomId = id;
            GameObject newPortal = GameObject.Instantiate(portalMarkerPrefab, this.gameObject.transform);
            previousPortal = newPortal;
            previousRoomId = id;
            portal.enabled = true;
            newPortal.SetActive(true);
            //portal.RefreshPortal();

            if (!canEnter)
            {
                Destroy(newPortal.GetComponent<BoxCollider>());
                var portalInternal = newPortal.transform.Find("PortalInternal(Clone)");
                if (portalInternal != null) Destroy(portalInternal.GetComponent<BoxCollider>());
            }
        }

        public void CheckPortalRoomId()
        {
            CheckRoomIdChange();
            SendCustomEventDelayedSeconds(nameof(CheckPortalRoomId), 10);
        }

        private void CheckRoomIdChange()
        {
            var inValid = false;
            if (Utilities.IsValid(previousPortal))
            {
                var targetPortal = previousPortal.GetComponent<VRCPortalMarker>();
                if (Utilities.IsValid(targetPortal))
                {
                    if (targetPortal.roomId != previousRoomId)
                    {
                        Debug.LogWarning($"[UdonPortal] RoomIDの変更を検出しました {targetPortal.roomId} => {previousRoomId}");
                        Message("<color=yellow>RoomIDの変更を検出しました</color>");
                        GenerateNewPortal(previousRoomId);
                    }
                }
                else
                {
                    inValid = true;
                }
            }
            else
            {
                inValid = true;
            }

            if (inValid && !string.IsNullOrEmpty(previousRoomId))
            {
                Debug.LogWarning("[UdonPortal] ポータルの変更を検出しました");
                Message("<color=yellow>ポータルの変更を検出しました</color>");
                GenerateNewPortal(previousRoomId);
            }
        }

        private void Message(string message)
        {
            if (Utilities.IsValid(messageManager))
            {
                messageManager.Message(message);
            }
        }

        #region 同期関係

        [UdonSynced] private string syncedId;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player.isLocal && !string.IsNullOrEmpty(syncedId))
            {
                GenerateNewPortal(syncedId);
            }
        }


        private void SyncRoomId(string roomId)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            syncedId = roomId;
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            GenerateNewPortal(syncedId);
        }

        #endregion
    }
}