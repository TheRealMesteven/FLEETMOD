using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLServer), "Internal_AttemptBlindJump")]
    internal class Internal_AttemptBlindJump
    {
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
