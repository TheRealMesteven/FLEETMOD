using System;
using HarmonyLib;

namespace FLEETMOD
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
