using System;
using HarmonyLib;

namespace FLEETMOD.Visuals
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
