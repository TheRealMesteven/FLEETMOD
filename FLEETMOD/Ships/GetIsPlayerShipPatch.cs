using System;
using HarmonyLib;

namespace FLEETMOD.Ships
{
    [HarmonyPatch(typeof(PLShipInfoBase), "GetIsPlayerShip")]
    internal class GetIsPlayerShipPatch
    {
        public static bool Prefix(PLShipInfoBase __instance, ref bool __result)
        {
            if (!MyVariables.isrunningmod) return true;
            if (__instance.TagID == -23)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
