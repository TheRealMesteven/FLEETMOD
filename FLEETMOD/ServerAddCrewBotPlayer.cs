using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200002A RID: 42
	[HarmonyPatch(typeof(PLServer), "ServerAddCrewBotPlayer")]
	internal class ServerAddCrewBotPlayer
	{
		// Token: 0x06000052 RID: 82 RVA: 0x000025CC File Offset: 0x000007CC
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
