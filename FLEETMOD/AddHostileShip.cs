using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipInfoBase), "AddHostileShip")]
	internal class AddHostileShip
	{
		public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase inShip, ref bool ___HostileShipAdded_NeedsResetForTargeting)
		{
            return true; // *Broken Original disable
            if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (inShip != null && !__instance.HostileShips.Contains(inShip.ShipID) && inShip.TagID != -23)
				{
					__instance.HostileShips.Add(inShip.ShipID);
					___HostileShipAdded_NeedsResetForTargeting = true;
				}
				return false;
			}
		}
	}
}
