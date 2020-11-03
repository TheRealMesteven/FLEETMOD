using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200001F RID: 31
	[HarmonyPatch(typeof(PLSpaceScrap), "Update")]
	internal class UpdateSpaceScrap
	{
		// Token: 0x0600003C RID: 60 RVA: 0x00005EF0 File Offset: 0x000040F0
		public static void Postfix(PLSpaceScrap __instance)
		{
			bool isrunningmod = MyVariables.isrunningmod;
			if (isrunningmod)
			{
				foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					bool flag = PhotonNetwork.isMasterClient && plshipInfoBase != null && plshipInfoBase.ExteriorTransformCached != null && (plshipInfoBase.ExteriorTransformCached.position - __instance.transform.position).sqrMagnitude < 6400f && !__instance.Collected && plshipInfoBase.TagID < -3;
					if (flag)
					{
						PLNetworkManager.Instance.LocalPlayer.FBSellAttemptsLeft = plshipInfoBase.ShipID;
						__instance.OnCollect();
					}
				}
			}
		}
	}
}
