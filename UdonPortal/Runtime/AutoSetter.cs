using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.UdonPortal
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AutoSetter : UdonSharpBehaviour
    {
        [SerializeField] private ControlPanel controlPanel;
        [SerializeField] private string worldId;
        [SerializeField] private string instanceId;
        [SerializeField] private UserOrGroup userOrGroup;
        [SerializeField] private InstanceType instanceType;
        [SerializeField] private string userId;
        [SerializeField] private GroupType groupType;
        [SerializeField] private string groupId;
        [SerializeField] private Region region;

        public void AutoSet()
        {
            if (!Utilities.IsValid(controlPanel))
            {
                Debug.LogError("ControlPanel is not set.");
                return;
            }
            if (userOrGroup == UserOrGroup.Group)
            {
                controlPanel.worldId.text = worldId;
                controlPanel.instanceId.text = instanceId;
                controlPanel.isGroup.isOn = true;
                controlPanel.groupId.text = groupId;
                controlPanel.groupType.value = (int)groupType;
                controlPanel.region.value = (int)region;
            }
            else if (userOrGroup == UserOrGroup.User)
            {
                controlPanel.worldId.text = worldId;
                controlPanel.instanceId.text = instanceId;
                controlPanel.isUser.isOn = true;
                controlPanel.instanceType.value = (int)instanceType;
                if (instanceType != InstanceType.Public)
                {
                    controlPanel.userId.text = userId;
                }
                controlPanel.region.value = (int)region;
            }
            else
            {
                return;
            }
        }
    }
}