using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLServer), "NetworkToggleWarpCharge")]
    internal class NetworkToggleWarpCharge
    {
        public static bool Prefix(PLServer __instance, int inShipID, int inWarpCharge, PhotonMessageInfo pmi)
        {
            if (!MyVariables.isrunningmod) return true;
            PLShipInfoBase shipFromID = PLEncounterManager.Instance.GetShipFromID(inShipID);
            PLPlayer playerForPhotonPlayer = PLServer.GetPlayerForPhotonPlayer(pmi.sender);
            if (shipFromID != null && playerForPhotonPlayer != null && playerForPhotonPlayer.TeamID == 0 && !playerForPhotonPlayer.IsBot)
            {
                // Allow Non-Modded Clients to interact with warp aside from Engaging / Skipping
                if (MyVariables.NonModded.Contains(playerForPhotonPlayer.GetPlayerID()) && (shipFromID.WarpChargeStage == EWarpChargeStage.E_WCS_READY || shipFromID.WarpChargeStage == EWarpChargeStage.E_WCS_ACTIVE))
                {
                    PLServer.Instance.photonView.RPC("AddNotification", pmi.sender, new object[]
                    {
                        "Sorry, You cannot use the Jump Computer without Fleet Mod.",
                        playerForPhotonPlayer.GetPlayerID(),
                        PLServer.Instance.GetEstimatedServerMs() + 3000,
                        true
                    });
                    return false;
                }

                // Alert Captain / Admiral (if there is no captain) of Activity
                if (PhotonNetwork.isMasterClient && shipFromID.WarpChargeStage != (EWarpChargeStage)inWarpCharge && shipFromID.GetIsPlayerShip())
                {
                    int CaptainID = PLNetworkManager.Instance.LocalPlayerID;
                    if (MyVariables.ShipHasCaptain(inShipID))
                    {
                        CaptainID = MyVariables.GetShipCaptain(inShipID);
                    }
                    PLPlayer cachedFriendlyPlayerOfClass = PLServer.Instance.GetPlayerFromPlayerID(CaptainID);
                    if (cachedFriendlyPlayerOfClass != null && playerForPhotonPlayer != cachedFriendlyPlayerOfClass)
                    {
                        if (inWarpCharge == 1)
                        {
                            PLServer.Instance.photonView.RPC("AddNotification", cachedFriendlyPlayerOfClass.GetPhotonPlayer(), new object[]
                            {
                                            "[PL] has started jump prep",
                                            playerForPhotonPlayer.GetPlayerID(),
                                            PLServer.Instance.GetEstimatedServerMs() + 6000,
                                            true
                            });
                        }
                        else if (inWarpCharge == 2)
                        {
                            PLServer.Instance.photonView.RPC("AddNotification", cachedFriendlyPlayerOfClass.GetPhotonPlayer(), new object[]
                            {
                                                "[PL] has paused the jump prep",
                                                playerForPhotonPlayer.GetPlayerID(),
                                                PLServer.Instance.GetEstimatedServerMs() + 6000,
                                                true
                            });
                        }
                    }
                }
                shipFromID.WarpChargeStage = (EWarpChargeStage)inWarpCharge;
            }
            return false;
        }
    }
}
