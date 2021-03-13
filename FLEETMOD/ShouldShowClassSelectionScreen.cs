using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLGame), "ShouldShowClassSelectionScreen")]
	internal class ShouldShowClassSelectionScreen
	{
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
