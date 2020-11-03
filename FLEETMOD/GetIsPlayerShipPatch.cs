using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000034 RID: 52
	[HarmonyPatch(typeof(PLShipInfoBase), "GetIsPlayerShip")]
	internal class GetIsPlayerShipPatch
	{
		// Token: 0x06000066 RID: 102 RVA: 0x0000907C File Offset: 0x0000727C
		public static bool Prefix(PLShipInfoBase __instance, ref bool __result)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = __instance.TagID == -23;
				if (flag2)
				{
					__result = true;
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}
	}
}
