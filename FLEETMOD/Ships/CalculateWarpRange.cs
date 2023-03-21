using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using HarmonyLib;
/*
 * Calculates warp range for fleet
 * 
 */
namespace FLEETMOD.Ships
{
    [HarmonyPatch(typeof(PLShipStats), "CalculateStats")]
    internal class CalculateWarpRange
    {
        public static void Postfix(ref ObscuredFloat ___m_WarpRange, PLShipStats __instance)
        {
            if (MyVariables.isrunningmod)
            {
                if (PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.PlayerLifeTime > 3f)
                {
                    foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
                    {
                        //PulsarModLoader.Utilities.Logger.Info($"{PLNetworkManager.Instance.LocalPlayer.GetPlayerName()} {__instance.Ship.ShipNameValue} | | {plshipInfoBase.MyStats.Ship.ShipNameValue}  {___m_WarpRange}");
                        if (plshipInfoBase != null && plshipInfoBase.GetLifetime() > 3f && plshipInfoBase.TagID == -23)
                        {
                            if (plshipInfoBase.MyWarpDrive == null)
                            {
                                ___m_WarpRange = 0f;
                                continue;
                            }
                            if (plshipInfoBase.MyWarpDrive.WarpRange < ___m_WarpRange)
                            {
                                ___m_WarpRange = plshipInfoBase.MyWarpDrive.WarpRange;
                            }
                        }
                    }
                    return;
                }
            }   
		}
	}
}
