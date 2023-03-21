using System;
using HarmonyLib;

namespace FLEETMOD.Ships
{
    [HarmonyPatch(typeof(PLShipInfoBase), "AddHostileShip")]
    internal class AddHostileShip
    {
        /// <summary>
        /// Makes Fleet ships identified as friendly to fleet ships.
        /// </summary>
        public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase inShip, ref bool ___HostileShipAdded_NeedsResetForTargeting)
        {
            if (!MyVariables.isrunningmod) return true;
            if (inShip != null && __instance != null && inShip != __instance && !__instance.HostileShips.Contains(inShip.ShipID) && !(inShip.GetIsPlayerShip() && __instance.GetIsPlayerShip()))
            {
                __instance.HostileShips.Add(inShip.ShipID);
                ___HostileShipAdded_NeedsResetForTargeting = true;
            }
            return false;
        }
    }
}
