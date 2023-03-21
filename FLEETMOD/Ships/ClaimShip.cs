using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Ships
{
    [HarmonyPatch(typeof(PLServer), "ClaimShip")]
    internal class ClaimShip
    {
        public static bool Prefix(PLServer __instance, int inShipID)
        {
            if (!MyVariables.isrunningmod) return true;
            else
            {
                foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                {
                    if (plplayer != null && plplayer.IsBot && plplayer.StartingShip == PLEncounterManager.Instance.GetShipFromID(inShipID))
                    {
                        plplayer.StartingShip = null;
                    }
                }
                if (!PhotonNetwork.isMasterClient)  return false;
                else
                {
                    if (PLEncounterManager.Instance.GetShipFromID(inShipID).TagID != -23 && PLEncounterManager.Instance.GetShipFromID(inShipID).TeamID == 1)
                    {
                        
                        PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                        {
                            PLEncounterManager.Instance.GetShipFromID(inShipID).ShipNameValue + " Is Now A Neutral Ship.",
                            Color.green,
                            0,
                            "SHIP"
                        });
                        PLEncounterManager.Instance.GetShipFromID(inShipID).TeamID = -1;
                    }
                    else
                    {
                        if (MyVariables.Fleet.Count < MyVariables.shipcount)
                        {
                            if (PLEncounterManager.Instance.GetShipFromID(inShipID).TagID != -23 && PLEncounterManager.Instance.GetShipFromID(inShipID).TeamID == -1)
                            {
                                PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                                {
                                PLEncounterManager.Instance.GetShipFromID(inShipID).ShipNameValue + " Is Now A Claimed Ship.",
                                Color.green,
                                0,
                                "SHIP"
                                });
                                PLEncounterManager.Instance.GetShipFromID(inShipID).TeamID = 1;
                                PLEncounterManager.Instance.GetShipFromID(inShipID).TagID = -23;
                                foreach (PLShipInfo pLShipInfo in PLEncounterManager.Instance.AllShips.Values)
                                {
                                    try
                                    {
                                        if (pLShipInfo.TagID == -23)
                                        {
                                            if (pLShipInfo.CaptainTargetedSpaceTargetID == inShipID)
                                            {
                                                pLShipInfo.CaptainTargetedSpaceTargetID = pLShipInfo.ShipID;
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }
    }
}
