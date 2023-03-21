using System;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD.ModMessages
{
    internal class HostUpdateVariables : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (PhotonNetwork.isMasterClient && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                PulsarModLoader.ModMessage.SendRPC(Plugin.harmonyIden, "FLEETMOD.ModMessages.ServerUpdateVariables", sender.sender, new object[]{
                    MyVariables.shipfriendlyfire,
                    MyVariables.recentfriendlyfire,
                    MyVariables.survivalBonusDict, // Sending healthBonus dictionary from host to clients
                    MyVariables.Modded,
                    MyVariables.NonModded,
                });
            }
        }
    }
}
