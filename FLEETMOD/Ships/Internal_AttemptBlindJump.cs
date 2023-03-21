using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Ships
{
    [HarmonyPatch(typeof(PLServer), "Internal_AttemptBlindJump")]
    internal class Internal_AttemptBlindJump
    {
        /// <summary>
        /// If Admiral Blind Jumps another ship, it self destructs.
        /// Otherwise, blind jump like normal.
        /// </summary>
        public static bool Prefix(int inShipID)
        {
            if (!MyVariables.isrunningmod) return true;
            if (PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID != inShipID && PhotonNetwork.isMasterClient)
            {
                PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                {
                    PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
                    0
                });
                PLEncounterManager.Instance.GetShipFromID(inShipID).DestroySelf(PLEncounterManager.Instance.GetShipFromID(inShipID));
                UnityEngine.Object.Destroy(PLEncounterManager.Instance.GetShipFromID(inShipID).gameObject);
                return false;
            }
            return true;
        }
    }
}
