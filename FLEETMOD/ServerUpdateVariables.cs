using System;
using System.Collections.Generic;
using HarmonyLib;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD
{   
    internal class ServerUpdateVariables : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            MyVariables.shipfriendlyfire = (bool)arguments[0];
            MyVariables.recentfriendlyfire = (bool)arguments[1];
            MyVariables.survivalBonusDict = (Dictionary<int, int>)arguments[2]; //Reading healthBonus info from host
            //MyVariables.warprange = (float)arguments[1];
        }
    }
    [HarmonyPatch(typeof(PLServer), "LoginMessage")] //Initial Sync. If multiple messages exist consider making new Modmessage for initial sync
    class LoginMessagePatch
    {
        static void Postfix(PhotonPlayer newPhotonPlayer)
        {
            if (PhotonNetwork.isMasterClient)
            {
                /*
                PulsarPluginLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.SyncCrewIDs", newPhotonPlayer, new object[] {
                    Global.Fleet,
                    Global.PlayerCrewList
                });
                */
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "Start")]
    class StartPatch
    { /// Initial Patch creating the dictionaries and lists.
        static void Postfix()
        {
            /*MyVariables.Fleet = new List<PLShipInfo>();
            MyVariables.FleetmodPlayer = new List<int>();
            MyVariables.FleetmodPhoton = new List<PhotonPlayer>();*/
            MyVariables.ShipCrews = new Dictionary<PLShipInfo, int>();
            MyVariables.survivalBonusDict = new Dictionary<int, int>();
            if (PhotonNetwork.isMasterClient)
            {
                MyVariables.UnModdedCrews = new Dictionary<int, int>();
            }
            //MyVariables.FleetmodPlayer.Add(PLNetworkManager.Instance.LocalPlayer.GetPlayerID());           
        }
    }
}
