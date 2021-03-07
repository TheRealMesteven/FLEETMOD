using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLMissionObjective_ReachSectorOfType), "GetNumEnemiesLeft")]
	internal class GetNumEnemiesLeft
	{
		public static bool Prefix(ref int __result)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				int num = 0;
				foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					if (plshipInfoBase != null && plshipInfoBase.TeamID == 1 && plshipInfoBase.TagID != -23)
					{
						num++;
					}
				}
				foreach (PLPawnBase plpawnBase in PLGameStatic.Instance.AllPawnBases)
				{
					if (plpawnBase != null && !plpawnBase.IsDead && (plpawnBase.MyPlayer == null || plpawnBase.MyPlayer.TeamID == 1))
					{
						num++;
					}
				}
				__result = num;
				result = false;
			}
			return result;
		}
	}
}
