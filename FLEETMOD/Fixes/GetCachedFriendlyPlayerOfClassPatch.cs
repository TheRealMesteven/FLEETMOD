using System;
using HarmonyLib;

namespace FLEETMOD.Fixes
{
    [HarmonyPatch(typeof(PLServer), "GetCachedFriendlyPlayerOfClass", new Type[]
    {
        typeof(int)
    })]
    internal class GetCachedFriendlyPlayerOfClassPatch
    {
        // Rework to get Crew Friendly Player Of Class?
        public static bool Prefix(PLServer __instance, ref int inClass, ref PLPlayer __result)
        {
            if (!Variables.isrunningmod) return true;
            if (inClass == 0)
            {
                __result = PLServer.Instance.GetPlayerFromPlayerID(0);
                return false;
            }
            else
            {
                foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                {
                    if (plplayer != null && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.StartingShip != null && plplayer.StartingShip == PLNetworkManager.Instance.LocalPlayer.StartingShip && plplayer.StartingShip != null && plplayer.GetClassID() == inClass && inClass != 0 && __instance != null)
                    {
                        __result = plplayer;
                        break;
                    }
                }
                return false;
            }
        }
    }
}
