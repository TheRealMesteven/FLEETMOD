using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000010 RID: 16
	[HarmonyPatch(typeof(PLWarpDriveScreen), "JumpBtnClick")]
	internal class JumpBtnClick
	{
		// Token: 0x0600001B RID: 27 RVA: 0x000044B4 File Offset: 0x000026B4
		public static bool Prefix(PLWarpDriveScreen __instance)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
            PLSectorInfo map = PLStarmap.Instance.CurrentShipPath[1];
			if (flag)
			{
				result = true;
			}
			else
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					bool flag2 = plshipInfoBase.TagID < -3 && plshipInfoBase != null;
					if (flag2)
					{
						bool flag3 = PLServer.Instance.m_ShipCourseGoals.Count > 0 && plshipInfoBase.WarpTargetID != map.ID;
						if (flag3)
						{
							num++;
						}
						bool flag4 = plshipInfoBase.WarpChargeStage != EWarpChargeStage.E_WCS_READY;
						if (flag4)
						{
							num2++;
						}
						bool flag5 = plshipInfoBase.NumberOfFuelCapsules < 1;
						if (flag5)
						{
							num3++;
						}
					}
				}
				bool flag6 = num == 0 && num2 == 0 && num3 == 0 && __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID != -1 && __instance.MyScreenHubBase.OptionalShipInfo.NumberOfFuelCapsules > 0 && PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) < 0.5f;
				bool flag7 = __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip() && !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard;
				if (flag7)
				{
					bool flag8 = false;
					EWarpChargeStage ewarpChargeStage = EWarpChargeStage.E_WCS_COLD_START;
					switch (__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage)
					{
					case EWarpChargeStage.E_WCS_PREPPING:
						ewarpChargeStage = EWarpChargeStage.E_WCS_PAUSED;
						flag8 = true;
						break;
					case EWarpChargeStage.E_WCS_PAUSED:
						ewarpChargeStage = EWarpChargeStage.E_WCS_PREPPING;
						flag8 = true;
						break;
					case EWarpChargeStage.E_WCS_READY:
					{
						bool flag9 = flag6 && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0;
						if (flag9)
						{
							flag8 = true;
							bool flag10 = PLNetworkManager.Instance.LocalPlayer != null;
							if (flag10)
							{
								PLServer.Instance.photonView.RPC("CPEI_HandleActivateWarpDrive", PhotonTargets.MasterClient, new object[]
								{
									__instance.MyScreenHubBase.OptionalShipInfo.ShipID,
									__instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID,
									PLNetworkManager.Instance.LocalPlayer.GetPlayerID()
								});
							}
							ewarpChargeStage = EWarpChargeStage.E_WCS_ACTIVE;
						}
						break;
					}
					case EWarpChargeStage.E_WCS_ACTIVE:
					{
						bool flag11 = PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0;
						if (flag11)
						{
							PLInGameUI.Instance.WarpSkipButtonClicked();
						}
						break;
					}
					default:
						ewarpChargeStage = EWarpChargeStage.E_WCS_PREPPING;
						flag8 = true;
						break;
					}
					bool flag12 = flag8;
					if (flag12)
					{
						bool flag13 = !PhotonNetwork.isMasterClient;
						if (flag13)
						{
							__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage = ewarpChargeStage;
						}
						PLServer.Instance.QueueRPC("NetworkToggleWarpCharge", PhotonTargets.MasterClient, new object[]
						{
							__instance.MyScreenHubBase.OptionalShipInfo.ShipID,
							(int)ewarpChargeStage
						});
					}
				}
				result = false;
			}
			return result;
		}
	}
}
