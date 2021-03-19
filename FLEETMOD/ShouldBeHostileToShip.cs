using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipInfoBase), "ShouldBeHostileToShip")]
	internal class ShouldBeHostileToShip
    {
		public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase inShip)
		{
            return true; // *Broken Original disable
            if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				bool flag2 = inShip == __instance || (inShip.GetIsPlayerShip() && __instance.GetIsPlayerShip());
				return !(inShip == __instance || (inShip.GetIsPlayerShip() && __instance.GetIsPlayerShip()));
			}
		}
	}
}
