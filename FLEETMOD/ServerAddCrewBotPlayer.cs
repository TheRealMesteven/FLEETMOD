using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "ServerAddCrewBotPlayer")]
	internal class ServerAddCrewBotPlayer
	{
		public static bool Prefix()
		{
            return true; // *Broken Original disable
            return !MyVariables.isrunningmod;
		}
	}
}
