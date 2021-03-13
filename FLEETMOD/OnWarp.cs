using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000039 RID: 57
	[HarmonyPatch(typeof(PLShipInfoBase), "OnWarp")]
	internal class OnWarp
	{
		// Token: 0x06000070 RID: 112 RVA: 0x000094AC File Offset: 0x000076AC
		public static bool Prefix(PLShipInfoBase __instance)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip == __instance as PLShipInfo)
				{
					foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
					{
						if (plshipInfoBase.GetIsPlayerShip() && plshipInfoBase != null && !plshipInfoBase.InWarp && plshipInfoBase != __instance)
						{
							foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
							{
								if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.GetPhotonPlayer().GetScore() == plshipInfoBase.ShipID)
								{
									plplayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
									{
										plshipInfoBase.MyTLI.SubHubID,
										0
									});
								}
							}
							PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
							{
								PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
								0
							});
                            plshipInfoBase.WarpTravelDist = 1f;
							plshipInfoBase.WarpTargetID = PLEncounterManager.Instance.PlayerShip.WarpTargetID;
							plshipInfoBase.WarpTravelPercent = 0f;
							plshipInfoBase.InWarp = true;
							plshipInfoBase.OnWarp(plshipInfoBase.WarpTargetID);
							plshipInfoBase.LastBeginWarpServerTime = PLEncounterManager.Instance.PlayerShip.LastBeginWarpServerTime;
							plshipInfoBase.WarpChargeStage = EWarpChargeStage.E_WCS_ACTIVE;
							PLWarpDrive shipComponent = plshipInfoBase.MyStats.GetShipComponent<PLWarpDrive>(ESlotType.E_COMP_WARP, false);
							if (shipComponent != null)
							{
								shipComponent.OnWarpTo(PLEncounterManager.Instance.PlayerShip.WarpTargetID);
							}
							plshipInfoBase.ClearSendQueue();
							PLShipInfo plshipInfo = plshipInfoBase as PLShipInfo;
							if (plshipInfo != null)
							{
								plshipInfo.BlindJumpUnlocked = false;
							}
							plshipInfoBase.NumberOfFuelCapsules--;
							plshipInfoBase.NumberOfFuelCapsules = Mathf.Clamp(plshipInfoBase.NumberOfFuelCapsules, 0, 200);
							plshipInfoBase.AlertLevel = 0;
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
