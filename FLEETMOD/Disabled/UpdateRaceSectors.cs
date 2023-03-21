using System;
using HarmonyLib;

namespace FLEETMOD.Disabled
{
	[HarmonyPatch(typeof(PLGalaxy), "UpdateRaceSectors")]
	internal class UpdateRaceSectors
	{
		public static bool Prefix()
		{
            return !Variables.isrunningmod;
		}
	}
}
