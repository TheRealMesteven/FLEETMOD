using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipInfoBase), "GetIsPlayerShip")]
	internal class GetIsPlayerShipPatch
	{
		public static bool Prefix(PLShipInfoBase __instance, ref bool __result)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (__instance.TagID == -23)
				{
					__result = true;
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
