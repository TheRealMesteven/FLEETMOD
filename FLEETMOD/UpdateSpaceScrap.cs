using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLSpaceScrap), "Update")]
	internal class UpdateSpaceScrap
	{
		public static void Postfix(PLSpaceScrap __instance)
		{
            return; // *Broken Original disable
            if (MyVariables.isrunningmod)
			{
				foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					if (PhotonNetwork.isMasterClient && plshipInfoBase != null && plshipInfoBase.ExteriorTransformCached != null && (plshipInfoBase.ExteriorTransformCached.position - __instance.transform.position).sqrMagnitude < 6400f && !__instance.Collected && plshipInfoBase.TagID < -3)
					{
						PLNetworkManager.Instance.LocalPlayer.FBSellAttemptsLeft = plshipInfoBase.ShipID;
						__instance.OnCollect();
					}
				}
			}
		}
	}
}
