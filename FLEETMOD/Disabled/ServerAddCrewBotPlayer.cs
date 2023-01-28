using System;
using HarmonyLib;

namespace FLEETMOD.Disabled
{
	[HarmonyPatch(typeof(PLServer), "ServerAddCrewBotPlayer")]
	internal class ServerAddCrewBotPlayer
	{
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
