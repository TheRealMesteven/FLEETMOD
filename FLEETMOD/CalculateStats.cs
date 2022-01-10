using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using HarmonyLib;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLShipStats), "CalculateStats")]
    internal class CalculateStats
    {
        public static void Postfix(ref ObscuredFloat ___m_WarpRange, PLShipStats __instance)
        {
            if (MyVariables.isrunningmod)
            {
                /*
                if (PLServer.Instance != null && __instance.Ship != null && __instance.Ship.TagID == -23 && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLNetworkManager.Instance.LocalPlayer.PlayerLifeTime > 3f)
                {
                    PulsarModLoader.Utilities.Logger.Info($"{PLNetworkManager.Instance.LocalPlayer.GetPlayerName()} {__instance.Ship.ShipNameValue}  |  {___m_WarpRange}");
                    float LowestWarpRange = __instance.Ship.MyStats.WarpRange;
                    foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
                    {
                        if (plshipInfoBase != null && plshipInfoBase.TagID == -23 && plshipInfoBase.MyStats.WarpRange < LowestWarpRange)
                        {
                            PulsarModLoader.Utilities.Logger.Info($"{PLNetworkManager.Instance.LocalPlayer.GetPlayerName()} Updated Range {___m_WarpRange}");
                            LowestWarpRange = plshipInfoBase.MyStats.WarpRange;
                        }
                    }
                    ___m_WarpRange = LowestWarpRange;
                }
                */
                if (PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.PlayerLifeTime > 3f)
                {
                    foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
                    {
                        //PulsarModLoader.Utilities.Logger.Info($"{PLNetworkManager.Instance.LocalPlayer.GetPlayerName()} {__instance.Ship.ShipNameValue} | | {plshipInfoBase.MyStats.Ship.ShipNameValue}  {___m_WarpRange}");
                        if (plshipInfoBase != null && plshipInfoBase.GetLifetime() > 3f && plshipInfoBase.TagID == -23)
                        {
                            if (plshipInfoBase.MyWarpDrive.WarpRange < ___m_WarpRange)
                            {
                                ___m_WarpRange = plshipInfoBase.MyWarpDrive.WarpRange;
                            }
                        }
                    }
                    return;
                }
            }
                            /*
                            if (__instance.Ship.MyWarpDrive.WarpRange < WarpRange || WarpRange == -1f)
                            {
                                WarpRange = __instance.Ship.MyWarpDrive.WarpRange < WarpRange
                            }
                        }
                        float LowestWarpRange = PLEncounterManager.Instance.PlayerShip.MyStats.WarpRange;

                        PulsarModLoader.Utilities.Logger.Info("[FMMM] ----------------------------");
                        foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
                        {

                            if (plshipInfoBase != null && plshipInfoBase.TagID == -23)
                            {
                                PulsarModLoader.Utilities.Logger.Info($"[FMMM] Warp Range: {plshipInfoBase.MyStats.WarpRange}");
                                if (plshipInfoBase.MyStats.WarpRange < LowestWarpRange)
                                {
                                    LowestWarpRange = plshipInfoBase.MyStats.WarpRange;
                                    PulsarModLoader.Utilities.Logger.Info($"[FMMM] Warp Range Reduced To: {LowestWarpRange}");
                                }
                            }
                        }
                        ___m_WarpRange = LowestWarpRange;
                        PulsarModLoader.Utilities.Logger.Info($"[FMMM] Saved Warp Range: {___m_WarpRange}");

                    }/*    /// STARTED WORKING ON REWRITING WARP RANGE CALCULATION BUT NOT MUCH LUCK. PROBS LOOK INTO THE GAMES IMPLEMENTATION
                    bool flag = PLServer.Instance != null;
                    if (flag)
                    {
                        if (PhotonNetwork.isMasterClient)
                        {
                            float lowestrange = -1;
                            foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
                            {
                                if (plshipInfoBase != null && plshipInfoBase.TagID == -23)
                                {
                                    if (lowestrange > plshipInfoBase.MyStats.WarpRange || lowestrange == -1)
                                    {
                                        lowestrange = plshipInfoBase.MyStats.WarpRange;
                                    }
                                }
                            }
                            ___m_WarpRange = lowestrange;
                            MyVariables.warprange = ___m_WarpRange;
                        }
                        else
                        {
                            ___m_WarpRange = MyVariables.warprange;
                        }
                    }*/
                        
		}
	}
}
