using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLSpaceScrap), "OnCollect")]
	internal class OnCollect
	{
		public static bool Prefix(PLSpaceScrap __instance, ref int ___m_EncounterNetID)
		{
            return true; // *Broken Original disable
            if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (!__instance.Collected)
				{
					PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(PLNetworkManager.Instance.LocalPlayer.FBSellAttemptsLeft) as PLShipInfo;
					if (PLAcademyShipInfo.Instance != null)
					{
						plshipInfo = PLAcademyShipInfo.Instance;
					}
					if (PhotonNetwork.isMasterClient && plshipInfo != null && PLServer.Instance != null)
					{
						PLSlot slot = plshipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO);
						if (slot != null && (slot.Count < slot.MaxItems || plshipInfo.ShipTypeID == EShipType.E_ACADEMY))
						{
							__instance.Collected = true;
							PLServer.Instance.photonView.RPC("ScrapCollectedEffect", PhotonTargets.All, new object[]
							{
								__instance.transform.position
							});
							if (plshipInfo.ShipTypeID == EShipType.E_ACADEMY)
							{
								return false;
							}
							PLServer.Instance.photonView.RPC("ScrapLateCollected", PhotonTargets.All, new object[]
							{
								__instance.EncounterNetID
							});
							if (__instance.IsSpecificComponentScrap)
							{
								plshipInfo.MyStats.AddShipComponent(PLWare.CreateFromHash(1, __instance.SpecificComponent_CompHash) as PLShipComponent, -1, ESlotType.E_COMP_CARGO);
								return false;
							}
							if (__instance.CanGiveComponent)
							{
								PLRand plrand = new PLRand(PLServer.Instance.GalaxySeed + PLServer.Instance.GetCurrentHubID() + ___m_EncounterNetID);
								int num = plrand.Next(0, 200);
								if (PLEncounterManager.Instance.PlayerShip.ShipTypeID == EShipType.E_CARRIER)
								{
									num = plrand.Next(0, 75);
								}
								Mathf.RoundToInt(Mathf.Pow(plrand.Next(0f, 1f), 4f) * PLServer.Instance.ChaosLevel);
								PLShipComponent plshipComponent = null;
								if (num < 50 && __instance.SpecificComponent_CompHash != -1)
								{
									plshipComponent = PLShipComponent.CreateShipComponentFromHash(__instance.SpecificComponent_CompHash, null);
								}
								if (plshipComponent == null)
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
				return false;
			}
		}
	}
}
