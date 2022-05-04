using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLWarpDriveScreen), "JumpBtnClick")]
	internal class JumpBtnClick
	{
		public static bool Prefix(PLWarpDriveScreen __instance)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				int UnalignedShips = 0;
				int UnchargedShips = 0;
				int UnFueledShips = 0;
				foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					if (plshipInfoBase.TagID < -3 && plshipInfoBase != null)
					{
						if (PLServer.Instance.m_ShipCourseGoals.Count > 0)
						{
                            if (PLServer.Instance.ClientHasFullStarmap && plshipInfoBase.WarpTargetID != PLStarmap.Instance.CurrentShipPath[1].ID)
                            {
                                UnalignedShips++;
                            }
                        }
                        else
                        {
                            UnalignedShips = 1;
						}
						if (plshipInfoBase.WarpChargeStage != EWarpChargeStage.E_WCS_READY)
						{
                            UnchargedShips++;
						}
						if (plshipInfoBase.NumberOfFuelCapsules < 1)
						{
                            UnFueledShips++;
						}
					}
				}
				bool flag6 = UnalignedShips == 0 && UnchargedShips == 0 && UnFueledShips == 0 && __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID != -1 && __instance.MyScreenHubBase.OptionalShipInfo.NumberOfFuelCapsules > 0 && PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) < 0.5f;
				if (__instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip() && !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard)
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
						if (flag6)
						{
							flag8 = true;
							if (PLNetworkManager.Instance.LocalPlayer != null)
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
						if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
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
					if (flag8)
					{
						if (!PhotonNetwork.isMasterClient)
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
