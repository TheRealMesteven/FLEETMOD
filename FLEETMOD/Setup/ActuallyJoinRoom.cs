using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.MPModChecks;
using PulsarModLoader.SaveData;
using PulsarModLoader.Utilities;
using UnityEngine;

namespace FLEETMOD.Setup
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
                if ((string)room.CustomProperties["Ship_Type"] == Mod.myversion)
                {
                    Variables.isrunningmod = false;
                    PLNetworkManager.Instance.JoinRoom(room);
                    PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
                    PLLoader.Instance.IsWaitingOnNetwork = true;
                }
                if (!room.CustomProperties["Ship_Type"].ToString().Contains("FLEETMOD"))
                {
                    Variables.isrunningmod = false;
                    PLNetworkManager.Instance.JoinRoom(room);
                    PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
                    PLLoader.Instance.IsWaitingOnNetwork = true;
                }
                if ((string)room.CustomProperties["Ship_Type"] != Mod.myversion && room.CustomProperties["Ship_Type"].ToString().Contains("."))
                {
                    PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Sorry, This Server Is Running FleetMod Version " + (string)room.CustomProperties["Ship_Type"] + "\n You Have Version " + Mod.myversion));
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
                if (PulsarModLoader.MPModChecks.MPModCheckManager.Instance.NetworkedPeerHasMod(playerAtID.GetPhotonPlayer(), Mod.harmonyIden))
                {
                    Variables.Modded.Add(inID);
                }
                else
                {
                    Variables.NonModded.Add(inID);
                }
                Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, "[NEW PLAYER] - Update Mod Message");
                ModMessages.ServerUpdateVariables.UpdateClients();
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
                Variables.isrunningmod = true;
            }
        }
    }
}
