using HarmonyLib;
using System.Reflection;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLCaptainScreen), "ShipCanBeClaimed")]
    class ShipCanBeClaimedPatch //Replaces original with code that checks if the ship is a player/fleet ship, then runs original screen
    {
        static bool Prefix(PLCaptainScreen __instance, ref bool __result)
        {
            if (__instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip())
            {
                __result = false;
            }
            else
            {
                foreach (int shipID in Global.Fleet.Values)
                {
                    if (__instance.MyScreenHubBase.OptionalShipInfo.ShipID == shipID)
                    {
                        __result = false;
                        return false;
                    }
                }

                //Invokes private method GetClamValues(out int, out int, out int). Effectively the following lines of code.
                //this.GetClaimValues(out int num, out int num2, out int num3);
                //return num >= num3;
                object[] parameters = new object[] { null, null, null };
                __instance.GetType().GetMethod("GetClaimValues", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, parameters);
                __result = (int)parameters[0] >= (int)parameters[2];
            }
            return false;
            
        }
    }
}
