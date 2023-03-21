using System;
using HarmonyLib;

namespace FLEETMOD.Ships
{
	[HarmonyPatch(typeof(PLShipInfoBase), "TakeDamage")]
	internal class TakeDamage
	{
		public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase attackingShip, ref float dmg)
		{
			if (!MyVariables.isrunningmod) return true;
			if (PLServer.GetCurrentSector().Name.Contains("W.D. HUB") || PLServer.GetCurrentSector().Name.Contains("Outpost 448") || PLServer.GetCurrentSector().Name.Contains("The Estate") || MyVariables.shipgodmode) return false;
            if (attackingShip != null && !MyVariables.shipfriendlyfire && __instance.TagID < -3 && attackingShip.TagID < -3 && attackingShip != __instance) return false;
			return true;
		}
	}
}
