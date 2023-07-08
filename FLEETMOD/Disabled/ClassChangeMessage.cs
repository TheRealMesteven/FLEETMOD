using System;
using HarmonyLib;

namespace FLEETMOD.Disabled
{
	[HarmonyPatch(typeof(PLServer), "ClassChangeMessage")]
	internal class ClassChangeMessage
	{
		public static bool Prefix()
		{
			return !Variables.isrunningmod;
		}
	}
}
