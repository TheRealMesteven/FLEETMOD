using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.MPModChecks;
using PulsarModLoader.SaveData;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLUIPlayMenu), "ActuallyJoinRoom")]
    internal class ActuallyJoinRoom
    {
        /// <summary>
        /// Used to update play-list with fleet name.
        /// Used to trigger Fleet.
        /// </summary>
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
    [HarmonyPatch(typeof(PLServer), "StartPlayer")]
    internal class StartPlayer
    {
        private static void Postfix(PLServer __instance, int inID)
        {
            PLPlayer playerAtID = __instance.GetPlayerFromPlayerID(inID);
            if (playerAtID != null && PhotonNetwork.isMasterClient)
            {
                if (playerAtID == PLNetworkManager.Instance.LocalPlayer)
                {
                    return;
                }
                if (PulsarModLoader.MPModChecks.MPModCheckManager.Instance.NetworkedPeerHasMod(playerAtID.GetPhotonPlayer(), Plugin.harmonyIden))
                {
                    MyVariables.Modded.Add(inID);
                }
                else
                {
                    MyVariables.NonModded.Add(inID);
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLGame), "Start")]
    internal class TriggerFleetmod
    {
        private static void Postfix()
        {
            if (PhotonNetwork.isMasterClient)
            {
                MyVariables.isrunningmod = true;
            }
        }
    }
    public class ActivateFleetmod : ModMessage
    {
        public static List<PhotonPlayer> PhotonClients;
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (sender.sender == PhotonNetwork.masterClient)
            {
                MyVariables.isrunningmod = true;
            }
        }
    }
}
