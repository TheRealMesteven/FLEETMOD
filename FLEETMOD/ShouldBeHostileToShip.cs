using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000038 RID: 56
	[HarmonyPatch(typeof(PLShipInfoBase), "ShouldBeHostileToShip")]
	internal class ShouldBeHostileToShip
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00009460 File Offset: 0x00007660
		public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase inShip)
		{
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
