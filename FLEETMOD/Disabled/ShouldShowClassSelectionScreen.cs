using System;
using HarmonyLib;

namespace FLEETMOD.Disabled
{
	[HarmonyPatch(typeof(PLGame), "ShouldShowClassSelectionScreen")]
	internal class ShouldShowClassSelectionScreen
	{
		public static bool Prefix()
		{
			return !Variables.isrunningmod;
		}
	}
}
