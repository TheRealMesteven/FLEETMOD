using System;
using HarmonyLib;

namespace FLEETMOD
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
