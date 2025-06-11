
using System;
using UdonSharp;
using UnityEngine;

namespace Nomlas.UdonPortal
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UdonPortalBase : UdonSharpBehaviour
    {
        protected static string Nonce()
        {
            return $"~nonce({Guid.NewGuid()})";
        }

        protected static string GetInstanceTypeString(InstanceType instanceType, string userId)
        {
            switch (instanceType)
            {
                case InstanceType.Public:
                    return "";
                case InstanceType.FriendsPlus:
                    return $"~hidden({userId})";
                case InstanceType.Friends:
                    return $"~friends({userId})";
                case InstanceType.InvitePlus:
                    return $"~private({userId})~canRequestInvite";
                case InstanceType.Invite:
                    return $"~private({userId})";
                default:
                    return "";
            }
        }

        protected static string GetGroupTypeString(GroupType groupType, string groupId)
        {
            switch (groupType)
            {
                case GroupType.Group:
                    return $"~group({groupId})~groupAccessType(members)";
                case GroupType.GroupPlus:
                    return $"~group({groupId})~groupAccessType(plus)";
                case GroupType.GroupPublic:
                    return $"~group({groupId})~groupAccessType(public)";
                default:
                    return "";
            }
        }

        protected static string GetRegion(Region region)
        {
            return $"region({GetRegionString(region)})";
        }

        protected static string GetRegionString(Region region)
        {
            switch (region)
            {
                case Region.us: return "us";
                case Region.use: return "use";
                case Region.eu: return "eu";
                case Region.jp: return "jp";
                default: return "eu";
            }
        }

        protected static string FString(string delimiter, string target)
        {
            return string.IsNullOrWhiteSpace(target) ? "" : (delimiter + target);
        }
    }
}