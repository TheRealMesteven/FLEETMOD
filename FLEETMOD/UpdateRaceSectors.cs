using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLGalaxy), "UpdateRaceSectors")]
	internal class UpdateRaceSectors
	{
		public static bool Prefix()
		{
            return true; // *Broken Original disable
            return !MyVariables.isrunningmod;
		}
	}
}
