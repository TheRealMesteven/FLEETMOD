using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(PLMissionObjective_ReachSectorOfType), "GetNumEnemiesLeft")]
	internal class GetNumEnemiesLeft
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000023E0 File Offset: 0x000005E0
		public static bool Prefix(ref int __result)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int num = 0;
				foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					bool flag2 = plshipInfoBase != null && plshipInfoBase.TeamID == 1 && plshipInfoBase.TagID != -23;
					if (flag2)
					{
						num++;
					}
				}
				foreach (PLPawnBase plpawnBase in PLGameStatic.Instance.AllPawnBases)
				{
					bool flag3 = plpawnBase != null && !plpawnBase.IsDead && (plpawnBase.MyPlayer == null || plpawnBase.MyPlayer.TeamID == 1);
					if (flag3)
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
