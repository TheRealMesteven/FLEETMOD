using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "ClassChangeMessage")]
	internal class ClassChangeMessage
	{
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
