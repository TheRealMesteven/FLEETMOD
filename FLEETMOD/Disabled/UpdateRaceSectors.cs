using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLGalaxy), "UpdateRaceSectors")]
	internal class UpdateRaceSectors
	{
		public static bool Prefix()
		{
            return !MyVariables.isrunningmod;
		}
	}
}
