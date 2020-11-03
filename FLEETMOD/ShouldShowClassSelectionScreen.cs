using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000021 RID: 33
	[HarmonyPatch(typeof(PLGame), "ShouldShowClassSelectionScreen")]
	internal class ShouldShowClassSelectionScreen
	{
		// Token: 0x06000040 RID: 64 RVA: 0x000025CC File Offset: 0x000007CC
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
