using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLServer), "CPEI_HandleActivateWarpDrive")]
    internal class CPEI_HandleActivateWarpDrive
    {
        /// <summary>
        /// Called by PLWarpDriveScreen in ActivateWarpDrive
        /// Sent to PhotonTargets.MasterClient
        /// </summary>
        public static bool Prefix(int shipID, int playerID)
        {
            if (!MyVariables.isrunningmod) return true;
            if (MyVariables.NonModded.Contains(playerID)) return false;

            // Notify Players of Warp
            PLServer.Instance.photonView.RPC("AddNotification", PhotonTargets.All, new object[]
            {
                PLServer.Instance.GetPlayerFromPlayerID(playerID).GetPlayerName(false) + " has engaged the warp! Heading to: " + PLEncounterManager.Instance.GetShipFromID(shipID).WarpTargetID.ToString(),
                playerID,
                PLServer.Instance.GetEstimatedServerMs() + 3000,
                true
            });

            // Begin Warp
            if (PLNetworkManager.Instance.LocalPlayer != null)
            {
                PLServer.Instance.photonView.RPC("NetworkBeginWarp", PhotonTargets.All, new object[]
                {
                    PLEncounterManager.Instance.PlayerShip.ShipID,
                    PLEncounterManager.Instance.GetShipFromID(shipID).WarpTargetID,
                    PLServer.Instance.GetEstimatedServerMs(),
                    -1
                });
            }
            return false;
        }
    }
}
