using HarmonyLib;
using System.Reflection;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLCaptainScreen), "ShipCanBeClaimed")]
    class ShipCanBeClaimedPatch
    {
        static bool Prefix(PLCaptainScreen __instance, ref bool __result)
        {
            if (!__instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip())
            {
                foreach(int shipID in Global.Fleet.Values)
                {
                    if(__instance.MyScreenHubBase.OptionalShipInfo.ShipID == shipID)
                    {
                        break;
                    }
                }
                __instance.GetType().GetMethod("ShipCanBeClaimed", BindingFlags: BindingFlags.Instance | BindingFlags.NonPublic)  GetClaimValues(out int num, out int num2, out int num3);
                return num >= num3;
            }
            __result = false;
            return false;
        }
    }
}
