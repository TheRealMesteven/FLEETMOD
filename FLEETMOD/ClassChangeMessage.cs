using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200002C RID: 44
	[HarmonyPatch(typeof(PLServer), "ClassChangeMessage")]
	internal class ClassChangeMessage
	{
		// Token: 0x06000056 RID: 86 RVA: 0x000025CC File Offset: 0x000007CC
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
