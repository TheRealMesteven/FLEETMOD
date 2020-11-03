using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200003A RID: 58
	[HarmonyPatch(typeof(PLShipInfoBase), "OnEndWarp")]
	internal class OnEndWarp
	{
		// Token: 0x06000072 RID: 114 RVA: 0x0000973C File Offset: 0x0000793C
		public static bool Prefix(PLShipInfoBase __instance)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip == __instance as PLShipInfo;
				if (flag2)
				{
					foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
					{
						bool flag3 = plshipInfoBase != null && plshipInfoBase.InWarp && plshipInfoBase.GetIsPlayerShip() && plshipInfoBase != __instance;
						if (flag3)
						{
							plshipInfoBase.SkipWarp();
							plshipInfoBase.InWarp = false;
							plshipInfoBase.WarpChargeStage = EWarpChargeStage.E_WCS_COLD_START;
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
