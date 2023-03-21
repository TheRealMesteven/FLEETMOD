using System;
using HarmonyLib;

namespace FLEETMOD.Disabled
{
	[HarmonyPatch(typeof(PLServer), "EnemyClaimShip")]
	internal class EnemyClaimShip
	{
		public static bool Prefix()
		{
			return !Variables.isrunningmod;
		}
	}
}
