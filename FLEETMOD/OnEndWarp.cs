using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipInfoBase), "OnEndWarp")]
	internal class OnEndWarp
	{
		public static bool Prefix(PLShipInfoBase __instance)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip == __instance as PLShipInfo)
				{
					foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
					{
						if (plshipInfoBase != null && plshipInfoBase.InWarp && plshipInfoBase.GetIsPlayerShip() && plshipInfoBase != __instance)
						{
							plshipInfoBase.SkipWarp();
							plshipInfoBase.InWarp = false;
                            plshipInfoBase.WarpChargeStage = EWarpChargeStage.E_WCS_COLD_START;
                        }
					}
				}
				return false;
			}
		}
	}
}
