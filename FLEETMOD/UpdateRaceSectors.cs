using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000009 RID: 9
	[HarmonyPatch(typeof(PLGalaxy), "UpdateRaceSectors")]
	internal class UpdateRaceSectors
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000025CC File Offset: 0x000007CC
		public static bool Prefix()
		{
            return !MyVariables.isrunningmod;
		}
	}
}
