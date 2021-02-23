using System;
using PulsarPluginLoader;
using UnityEngine;

namespace FLEETMOD
{
    // Token: 0x02000027 RID: 39
    internal class HostUpdateVariables : ModMessage
    {
        // Token: 0x0600004C RID: 76 RVA: 0x000069FC File Offset: 0x00004BFC
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (PhotonNetwork.isMasterClient && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                PulsarPluginLoader.ModMessage.SendRPC("Michael+Mest.Fleetmod", "FLEETMOD.ServerUpdateVariables", PhotonTargets.All, new object[]{
                    MyVariables.shipfriendlyfire,
                    //MyVariables.warprange,
                });
            }
        }
    }
}
