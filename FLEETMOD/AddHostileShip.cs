using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000035 RID: 53
	[HarmonyPatch(typeof(PLShipInfoBase), "AddHostileShip")]
	internal class AddHostileShip
	{
		// Token: 0x06000068 RID: 104 RVA: 0x000090B8 File Offset: 0x000072B8
		public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase inShip, ref bool ___HostileShipAdded_NeedsResetForTargeting)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = inShip != null && !__instance.HostileShips.Contains(inShip.ShipID) && inShip.TagID != -23;
				if (flag2)
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
