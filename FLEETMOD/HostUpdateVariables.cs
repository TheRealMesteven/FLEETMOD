using System;
using PulsarPluginLoader;
using UnityEngine;

namespace FLEETMOD
{
    internal class HostUpdateVariables : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (PhotonNetwork.isMasterClient && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                PulsarPluginLoader.ModMessage.SendRPC("Michael+Mest.Fleetmod", "FLEETMOD.ServerUpdateVariables", PhotonTargets.All, new object[]{
                    MyVariables.shipfriendlyfire,
                    MyVariables.recentfriendlyfire,
                    //MyVariables.warprange,
                });
            }
        }
    }
}
