using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x0200001E RID: 30
	[HarmonyPatch(typeof(PLSpaceScrap), "OnCollect")]
	internal class OnCollect
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00005C60 File Offset: 0x00003E60
		public static bool Prefix(PLSpaceScrap __instance, ref int ___m_EncounterNetID)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = !__instance.Collected;
				if (flag2)
				{
					PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(PLNetworkManager.Instance.LocalPlayer.FBSellAttemptsLeft) as PLShipInfo;
					bool flag3 = PLAcademyShipInfo.Instance != null;
					if (flag3)
					{
						plshipInfo = PLAcademyShipInfo.Instance;
					}
					bool flag4 = PhotonNetwork.isMasterClient && plshipInfo != null && PLServer.Instance != null;
					if (flag4)
					{
						PLSlot slot = plshipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO);
						bool flag5 = slot != null && (slot.Count < slot.MaxItems || plshipInfo.ShipTypeID == EShipType.E_ACADEMY);
						if (flag5)
						{
							__instance.Collected = true;
							PLServer.Instance.photonView.RPC("ScrapCollectedEffect", PhotonTargets.All, new object[]
							{
								__instance.transform.position
							});
							bool flag6 = plshipInfo.ShipTypeID == EShipType.E_ACADEMY;
							if (flag6)
							{
								return false;
							}
							PLServer.Instance.photonView.RPC("ScrapLateCollected", PhotonTargets.All, new object[]
							{
								__instance.EncounterNetID
							});
							bool isSpecificComponentScrap = __instance.IsSpecificComponentScrap;
							if (isSpecificComponentScrap)
							{
								plshipInfo.MyStats.AddShipComponent(PLWare.CreateFromHash(1, __instance.SpecificComponent_CompHash) as PLShipComponent, -1, ESlotType.E_COMP_CARGO);
								return false;
							}
							bool canGiveComponent = __instance.CanGiveComponent;
							if (canGiveComponent)
							{
								PLRand plrand = new PLRand(PLServer.Instance.GalaxySeed + PLServer.Instance.GetCurrentHubID() + ___m_EncounterNetID);
								int num = plrand.Next(0, 200);
								bool flag7 = PLEncounterManager.Instance.PlayerShip.ShipTypeID == EShipType.E_CARRIER;
								if (flag7)
								{
									num = plrand.Next(0, 75);
								}
								Mathf.RoundToInt(Mathf.Pow(plrand.Next(0f, 1f), 4f) * PLServer.Instance.ChaosLevel);
								PLShipComponent plshipComponent = null;
								bool flag8 = num < 50 && __instance.SpecificComponent_CompHash != -1;
								if (flag8)
								{
									plshipComponent = PLShipComponent.CreateShipComponentFromHash(__instance.SpecificComponent_CompHash, null);
								}
								bool flag9 = plshipComponent == null;
								if (flag9)
								{
									plshipComponent = new PLScrapCargo(0);
								}
								plshipInfo.MyStats.AddShipComponent(plshipComponent, -1, ESlotType.E_COMP_CARGO);
								return false;
							}
							plshipInfo.MyStats.AddShipComponent(new PLScrapCargo(0), -1, ESlotType.E_COMP_CARGO);
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
