using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200002F RID: 47
	[HarmonyPatch(typeof(PLServer), "EnemyClaimShip")]
	internal class EnemyClaimShip
	{
		// Token: 0x0600005C RID: 92 RVA: 0x000025CC File Offset: 0x000007CC
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
