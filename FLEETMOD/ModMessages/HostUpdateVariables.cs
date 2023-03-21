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
                PulsarModLoader.ModMessage.SendRPC(Mod.harmonyIden, "FLEETMOD.ModMessages.ServerUpdateVariables", sender.sender, new object[]{
                    Variables.shipfriendlyfire,
                    Variables.recentfriendlyfire,
                    Variables.survivalBonusDict, // Sending healthBonus dictionary from host to clients
                    Variables.Modded,
                    Variables.NonModded,
                });
            }
        }
    }
}
