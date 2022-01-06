using System;
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
            MyVariables.Fleet = new List<int>();
            MyVariables.EnabledFleetmod = new List<PLPlayer>();
            MyVariables.ShipCrews = new Dictionary<int, PLPlayer>();
            EnabledFleetmod.Add(PLNetworkManager.Instance.LocalPlayer);
        }
    }
}
