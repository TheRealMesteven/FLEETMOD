using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using static PulsarModLoader.Patches.HarmonyHelpers;
using static HarmonyLib.AccessTools;
using ExitGames.Client.Photon.LoadBalancing;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLHighRollersShipInfo), "OnLiarsDiceGameWin")]
    class HighRollersChips
    {
        public static void Postfix(PLHighRollersShipInfo __instance, PLPlayer player, PLLiarsDiceGame game)
        {
            if (!MyVariables.isrunningmod) return;
            if (player != null && player.StartingShip != null)
            {
                if (player.StartingShip != PLEncounterManager.Instance.PlayerShip && MyVariables.Fleet.ContainsKey(player.StartingShip.ShipID) && Array.IndexOf<PLLiarsDiceGame>(__instance.SmallGames, game) != -1)
                {
                    __instance.CrewChips++;
                }
                if (game == __instance.BigGame)
                {
                    __instance.FinalGameAvailable = false;
                    if (player.StartingShip != PLEncounterManager.Instance.PlayerShip && MyVariables.Fleet.ContainsKey(player.StartingShip.ShipID))
                    {
                        PLServer.Instance.CollectFragment(3);
                    }
                }
            }
        }
    }
}
