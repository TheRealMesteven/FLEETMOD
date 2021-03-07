using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipInfoBase), "GetIsPlayerShip")]
	internal class GetIsPlayerShipPatch
	{
		public static bool Prefix(PLShipInfoBase __instance, ref bool __result)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				if (__instance.TagID == -23)
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
