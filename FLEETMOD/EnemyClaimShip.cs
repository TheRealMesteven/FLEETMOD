using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "EnemyClaimShip")]
	internal class EnemyClaimShip
	{
		public static bool Prefix()
		{
			return !MyVariables.isrunningmod;
		}
	}
}
