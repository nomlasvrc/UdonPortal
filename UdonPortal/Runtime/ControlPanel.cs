
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.UdonPortal
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ControlPanel : MessageManager
    {
        [SerializeField] internal UdonPortal udonPortal;
        [Space]
        [SerializeField] internal TMP_InputField worldId;
        [SerializeField] internal TMP_InputField instanceId;
        [SerializeField] internal Toggle isUser;
        [SerializeField] internal Toggle isGroup;
        [SerializeField] internal TMP_InputField userId;
        [SerializeField] internal TMP_Dropdown instanceType;
        [SerializeField] internal TMP_InputField groupId;
        [SerializeField] internal TMP_Dropdown groupType;
        [SerializeField] internal TMP_Dropdown region;
        [Space]
        [SerializeField] internal TextMeshProUGUI tmpMessage;
        private UserOrGroup _UserOrGroup
        {
            get
            {
                var user = Utilities.IsValid(isUser) && isUser.isOn;
                var group = Utilities.IsValid(isGroup) && isGroup.isOn;
                if (user && group)
                {
                    return UserOrGroup.Both;
                }
                else if (user)
                {
                    return UserOrGroup.User;
                }
                else if (group)
                {
                    return UserOrGroup.Group;
                }
                else
                {
                    return UserOrGroup.Neither;
                }
            }
        }
        private float previousMessageTime;

        void Start()
        {
            TMPMessage("Hello!");
        }

        public void OnClickGenerate()
        {
            var worldIdValue = worldId.text;
            var instanceIdValue = instanceId.text;
            var userIdValue = userId.text;
            var instanceTypeValue = (InstanceType)instanceType.value;
            var groupIdValue = groupId.text;
            var groupTypeValue = (GroupType)groupType.value;
            var regionValue = (Region)region.value;

            if (string.IsNullOrWhiteSpace(worldIdValue))
            {
                TMPMessage("<color=red>World ID is empty.</color>");
                return;
            }
            if (string.IsNullOrWhiteSpace(instanceIdValue))
            {
                TMPMessage("<color=red>Instance ID is empty.</color>");
                return;
            }
            if (_UserOrGroup == UserOrGroup.User && string.IsNullOrWhiteSpace(userIdValue) && instanceTypeValue != InstanceType.Public)
            {
                TMPMessage("<color=red>User ID is empty.</color>");
                return;
            }
            if (_UserOrGroup == UserOrGroup.Group && string.IsNullOrWhiteSpace(groupIdValue))
            {
                TMPMessage("<color=red>Group ID is empty.</color>");
                return;
            }
            if (_UserOrGroup == UserOrGroup.User)
            {
                if (instanceTypeValue == InstanceType.Public)
                {
                    udonPortal.NewPortal(worldIdValue, instanceIdValue, regionValue);
                }
                else
                {
                    udonPortal.NewPortal(worldIdValue, instanceIdValue, userIdValue, instanceTypeValue, regionValue);
                }
            }
            else if (_UserOrGroup == UserOrGroup.Group)
            {
                udonPortal.NewPortal(worldIdValue, instanceIdValue, groupIdValue, groupTypeValue, regionValue);
            }
        }

        public void OnChangeInstanceType() //UserとGroupの切り替え時にも発火
        {
            if (_UserOrGroup != UserOrGroup.User) return;
            var instanceTypeValue = (InstanceType)instanceType.value;
            if (instanceTypeValue == InstanceType.Public)
            {
                userId.interactable = false;
            }
            else
            {
                userId.interactable = true;
            }
        }

        private void TMPMessage(string message)
        {
            tmpMessage.text = message;
            SetClearEvent();
        }

        public override void Message(string message)
        {
            TMPMessage(message);
        }

        private void SetClearEvent()
        {
            previousMessageTime = Time.time;
            SendCustomEventDelayedSeconds(nameof(ClearTMPMessage), 5);
        }

        public void ClearTMPMessage()
        {
            if (Time.time >= previousMessageTime + 4.8)
            {
                tmpMessage.text = "";
            }
        }
    }
}