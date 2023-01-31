using System;
using System.Collections.Generic;
using HarmonyLib;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLShipInfoBase), "OnEndWarp")]
    internal class OnEndWarp
    {
        public static bool Prefix(PLShipInfoBase __instance)
        {
            if (!MyVariables.isrunningmod) return true;
            if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip == __instance as PLShipInfo)
            {
                // Store the locations of NonModded Players for Teleporting
                Dictionary<int, PlayerPos> UnModdedPositions = new Dictionary<int, PlayerPos>(); // PlayerID, PlayerPos
                foreach (int playerid in MyVariables.NonModded)
                {
                    PLPlayer plplayer = PLServer.Instance.GetPlayerFromPlayerID(playerid);
                    if (plplayer != null && MyVariables.UnModdedCrews.ContainsKey(playerid))
                    {
                        PlayerPos Position = new PlayerPos
                        {
                            pos = plplayer.GetPawn().transform.position,
                            hubid = plplayer.MyCurrentTLI.SubHubID,
                            ttiid = plplayer.TTIID
                        };
                        UnModdedPositions.Add(playerid, Position);
                    }
                }

                // Skip warp for ALL fleet ships
                foreach (int plshipid in MyVariables.Fleet.Keys)
                {
                    PLShipInfoBase plshipInfoBase = PLEncounterManager.Instance.GetShipFromID(plshipid);
                    if (plshipInfoBase != null && plshipInfoBase.InWarp && plshipInfoBase.GetIsPlayerShip() && plshipInfoBase != __instance)
                    {
                        plshipInfoBase.SkipWarp();
                        plshipInfoBase.InWarp = false;
                        plshipInfoBase.WarpChargeStage = EWarpChargeStage.E_WCS_COLD_START;
                    }
                }

                // Create the FleetManager Dialog
                if (PLServer.GetCurrentSector().Name.Contains("W.D. HUB") || PLServer.GetCurrentSector().Name.Contains("Outpost 448") || PLServer.GetCurrentSector().Name.Contains("The Estate") || PLServer.GetCurrentSector().Name.Contains("Cornelia Station") || PLServer.GetCurrentSector().Name.Contains("The Burrow") || PLServer.GetCurrentSector().Name.Contains("The Harbor"))
                {
                    if (MyVariables.DialogGenerated != true)
                    {
                        var go = new UnityEngine.GameObject("FleetManager_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                        go.AddComponent<Interface.Dialogs.FleetManager>(); // Also TODO: Rename local vars...
                        MyVariables.DialogGenerated = true;
                    }
                }

                // Teleport NonModded Players now that ship is in warp
                foreach (KeyValuePair<int, PlayerPos> keyValuePair in UnModdedPositions) // Teleport unmodded players back to their positions when warping
                {
                    PLPlayer player = PLServer.Instance.GetPlayerFromPlayerID(keyValuePair.Key);
                    player.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                    {
                                keyValuePair.Value.hubid,
                                keyValuePair.Value.ttiid
                    });
                    player.photonView.RPC("RecallPawnToPos", PhotonTargets.All, new object[]
                    {
                                keyValuePair.Value.pos
                    });
                }
            }
            return false;
        }
    }
}
