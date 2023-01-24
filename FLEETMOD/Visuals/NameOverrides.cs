using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD.Visuals
{
    [HarmonyPatch(typeof(PLPawn), "GetName")]
    internal class CombatTargetNameOverride
    {
        public static bool Prefix(PLPawn __instance, ref string __result)
        {
            if (!MyVariables.isrunningmod) return true;
            __result = "";
            if (__instance.MyPlayer != null)
            {
                __result = $"{__instance.MyPlayer.StartingShip.ShipNameValue} • {__instance.MyPlayer.GetPlayerName(false)}";
            }
            return false;
        }
    }
}
