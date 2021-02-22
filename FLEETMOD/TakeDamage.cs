using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000037 RID: 55
	[HarmonyPatch(typeof(PLShipInfoBase), "TakeDamage")]
	internal class TakeDamage
	{
		// Token: 0x0600006C RID: 108 RVA: 0x000093C8 File Offset: 0x000075C8
		public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase attackingShip, ref float dmg)
		{
			bool flag = !MyVariables.isrunningmod;
			if (flag)
			{
				return true;
			}
			else
			{
				if (PLServer.GetCurrentSector().Name.Contains("W.D. HUB") || PLServer.GetCurrentSector().Name.Contains("Outpost 448") || PLServer.GetCurrentSector().Name.Contains("The Estate") || MyVariables.shipgodmode)
                {
                    return false;
                }
                else
                {
                    if (attackingShip != null && MyVariables.shipfriendlyfire && __instance.TagID < -3 && attackingShip.TagID < -3 && attackingShip != __instance)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
			}
		}
	}
}
