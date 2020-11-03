using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200001A RID: 26
	[HarmonyPatch(typeof(PLInGameUI), "GetTextOfCaptainOrderID")]
	internal class GetTextOfCaptainOrderID
	{
		// Token: 0x06000032 RID: 50 RVA: 0x000025CC File Offset: 0x000007CC
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
