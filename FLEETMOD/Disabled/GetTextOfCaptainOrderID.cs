using System;
using HarmonyLib;

namespace FLEETMOD.Disabled
{
	[HarmonyPatch(typeof(PLInGameUI), "GetTextOfCaptainOrderID")]
	internal class GetTextOfCaptainOrderID
	{
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
