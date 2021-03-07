using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipInfoBase), "AddHostileShip")]
	internal class AddHostileShip
	{
		public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase inShip, ref bool ___HostileShipAdded_NeedsResetForTargeting)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				if (inShip != null && !__instance.HostileShips.Contains(inShip.ShipID) && inShip.TagID != -23)
				{
					__instance.HostileShips.Add(inShip.ShipID);
					___HostileShipAdded_NeedsResetForTargeting = true;
				}
				result = false;
			}
			return result;
		}
	}
}
