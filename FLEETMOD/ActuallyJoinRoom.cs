using System;
using HarmonyLib;
using PulsarModLoader;
/*
 * Checks if player is running mod
 * Netcode for joining room with fleetmod running
 * 
 * 
 */
namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLUIPlayMenu), "ActuallyJoinRoom")]
    internal class ActuallyJoinRoom
    {
        public static bool Prefix(RoomInfo room)
        {
            if ((int)room.CustomProperties["CurrentPlayersPlusBots"] < (int)room.MaxPlayers)
            {
                if (room.CustomProperties.ContainsKey("SteamServerID"))
                {
                    if (!SteamManager.Initialized)
                    {
                        PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Failed to join crew! Can't join Secured game when not logged in to Steam!"));
                        return false;
                    }
                    uint num = (uint)((long)room.CustomProperties["SteamServerID"]);
                    PLNetworkManager.Instance.ClearSteamAuthSession(num);
                    PLNetworkManager.Instance.SetSteamAuthTicket(num);
                }
                if ((string)room.CustomProperties["Ship_Type"] == Plugin.myversion)
                {
                    MyVariables.isrunningmod = false;
                    PLNetworkManager.Instance.JoinRoom(room);
                    PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
                    PLLoader.Instance.IsWaitingOnNetwork = true;
                }
                if (!room.CustomProperties["Ship_Type"].ToString().Contains("FLEETMOD"))
                {
                    MyVariables.isrunningmod = false;
                    PLNetworkManager.Instance.JoinRoom(room);
                    PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
                    PLLoader.Instance.IsWaitingOnNetwork = true;
                }
                if ((string)room.CustomProperties["Ship_Type"] != Plugin.myversion && room.CustomProperties["Ship_Type"].ToString().Contains("."))
                {
                    PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Sorry, This Server Is Running FleetMod Version " + (string)room.CustomProperties["Ship_Type"] + "\n You Have Version " + Plugin.myversion));
                }
                return false;
            }
            else
            {
                PLTabMenu.Instance.TimedErrorMsg = "Couldn't join crew! It is full!";
                return false;
            }
        }
    }
    [HarmonyPatch(typeof(PLGame), "Start")]
    internal class TriggerFleetmod
    {
        private static void Postfix()
        {
            if (!PhotonNetwork.isMasterClient && !MyVariables.isrunningmod)
            {
                MyVariables.isrunningmod = false;
                ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ActivateFleetmod", PhotonTargets.MasterClient, new object[] { });
            }
            else
            {
                MyVariables.isrunningmod = true;
            }
        }
    }
    public class ActivateFleetmod : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (sender.sender == PhotonNetwork.masterClient)
            {
                MyVariables.isrunningmod = true;
            }
            else if (PhotonNetwork.isMasterClient && MyVariables.isrunningmod)
            {
                //MyVariables.FleetmodPhoton.Add(sender.sender);
                ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ActivateFleetmod", sender.sender, new object[] { });
            }
        }
    }
}
