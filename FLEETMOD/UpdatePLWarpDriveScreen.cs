using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(PLWarpDriveScreen), "Update")]
	internal class UpdatePLWarpDriveScreen
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00003B00 File Offset: 0x00001D00
		public static void Postfix(PLWarpDriveScreen __instance, ref UISprite ___JumpComputerPanel, ref UISprite ___WarpDrivePanel, ref UISprite ___m_BlockingTargetOnboardPanel, ref UILabel ___m_JumpButtonLabel, ref UILabel ___BlindJumpBtnLabel, ref UILabel ___BlindJumpWarning, ref UISprite ___BlindJumpBtn, ref float ___TargetAlpha_WarpPanel, ref UILabel ___m_JumpButtonLabelTop, ref UILabel ___m_BlockingTargetOnboardPanelTitle, ref UIPanel ___m_JumpButtonMask, ref UIPanel[] ___ChargeStage_BarMask, ref UILabel[] ___ChargeStage_Label, string[] ___ChargeStage_Name)
		{
			bool isrunningmod = MyVariables.isrunningmod;
			if (isrunningmod)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				string text = "";
				string str = "";
				string str2 = "";
                PLSectorInfo map = PLStarmap.Instance.CurrentShipPath[1];
                foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					bool flag = plshipInfoBase.TagID < -3 && plshipInfoBase != null && PLServer.Instance.m_ShipCourseGoals.Count > 0;
					if (flag)
					{
						bool flag2 = plshipInfoBase.WarpTargetID != map.ID;
						if (flag2)
						{
							num++;
							text = plshipInfoBase.ShipNameValue;
						}
						bool flag3 = plshipInfoBase.WarpChargeStage != EWarpChargeStage.E_WCS_READY;
						if (flag3)
						{
							num2++;
							str = plshipInfoBase.ShipNameValue;
						}
						bool flag4 = plshipInfoBase.NumberOfFuelCapsules < 1;
						if (flag4)
						{
							num3++;
							str2 = plshipInfoBase.ShipNameValue;
						}
					}
				}
				PLGlobal.SafeGameObjectSetActive(___WarpDrivePanel.gameObject, !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard && __instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage != EWarpChargeStage.E_WCS_ACTIVE && __instance.MyScreenHubBase.OptionalShipInfo.NumberOfFuelCapsules > 0 && !__instance.MyScreenHubBase.OptionalShipInfo.InWarp && !__instance.MyScreenHubBase.OptionalShipInfo.Abandoned);
				PLGlobal.SafeGameObjectSetActive(___JumpComputerPanel.gameObject, !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard);
				PLGlobal.SafeGameObjectSetActive(___m_BlockingTargetOnboardPanel.gameObject, __instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard);
				bool blockingCombatTargetOnboard = __instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard;
				if (blockingCombatTargetOnboard)
				{
					Color b = Color.Lerp(new Color(0.8f, 0f, 0f, 1f), new Color(0.65f, 0.65f, 0.65f), Time.time % 1f);
					___m_BlockingTargetOnboardPanelTitle.color = Color.Lerp(___m_BlockingTargetOnboardPanelTitle.color, b, Time.deltaTime * 2f);
				}
				bool flag5 = __instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard || __instance.MyScreenHubBase.OptionalShipInfo.HasVirusOfType(EVirusType.WARP_DISABLE) || PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) > 0.5f;
				if (flag5)
				{
					___BlindJumpBtnLabel.text = "ERROR";
					___BlindJumpBtnLabel.color = Color.black;
					___BlindJumpWarning.text = "ERROR: Blind Jump Unavailable!";
					___BlindJumpBtn.spriteName = "button_fill";
				}
				else
				{
					bool flag6 = __instance.MyScreenHubBase.OptionalShipInfo.BlindJumpUnlocked && PhotonNetwork.isMasterClient;
					if (flag6)
					{
						bool flag7 = PhotonNetwork.isMasterClient && __instance.MyScreenHubBase.OptionalShipInfo.ShipID == PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID;
						if (flag7)
						{
							___BlindJumpBtnLabel.text = "BLIND JUMP";
							___BlindJumpBtnLabel.color = Color.black;
							___BlindJumpWarning.text = "ADMIRAL - BLIND JUMP";
							___BlindJumpBtn.spriteName = "button_fill";
						}
						else
						{
							bool flag8 = PhotonNetwork.isMasterClient && __instance.MyScreenHubBase.OptionalShipInfo.ShipID != PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID;
							if (flag8)
							{
								___BlindJumpBtnLabel.text = "DESTROY SHIP";
								___BlindJumpBtnLabel.color = Color.black;
								___BlindJumpWarning.text = "For Emergency Use By The Admiral";
								___BlindJumpBtn.spriteName = "button_fill";
							}
						}
					}
					else
					{
						bool flag9 = PhotonNetwork.isMasterClient && !__instance.MyScreenHubBase.OptionalShipInfo.BlindJumpUnlocked && __instance.MyScreenHubBase.OptionalShipInfo.ShipID != PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID;
						if (flag9)
						{
							___BlindJumpWarning.text = "ADMIRAL - DESTROY SHIP";
							___BlindJumpBtn.spriteName = "button";
							___BlindJumpBtnLabel.color = Color.red;
							___BlindJumpBtnLabel.text = "UNLOCK";
						}
						else
						{
							bool flag10 = PhotonNetwork.isMasterClient && !__instance.MyScreenHubBase.OptionalShipInfo.BlindJumpUnlocked;
							if (flag10)
							{
								___BlindJumpWarning.text = "ADMIRAL - BLIND JUMP";
								___BlindJumpBtn.spriteName = "button";
								___BlindJumpBtnLabel.color = Color.red;
								___BlindJumpBtnLabel.text = "UNLOCK";
							}
							else
							{
								bool flag11 = !PhotonNetwork.isMasterClient;
								if (flag11)
								{
									___BlindJumpWarning.text = "Target Sector";
									bool flag12 = PLServer.Instance.m_ShipCourseGoals.Count > 0;
									if (flag12)
									{
										___BlindJumpBtnLabel.text = map.ID.ToString();
									}
									else
									{
										___BlindJumpBtnLabel.text = "No Course Set";
									}
								}
							}
						}
					}
				}
				float warpChargePercentTotal = __instance.MyScreenHubBase.OptionalShipInfo.GetWarpChargePercentTotal();
				___m_JumpButtonMask.clipOffset = new Vector2((warpChargePercentTotal - 1f) * ___m_JumpButtonMask.width, 0f);
				switch (__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage)
				{
				case EWarpChargeStage.E_WCS_PREPPING:
					___TargetAlpha_WarpPanel = 1f;
					___m_JumpButtonLabel.text = "Charging Warp Drive";
					___m_JumpButtonLabelTop.text = "Charging Warp Drive";
					break;
				case EWarpChargeStage.E_WCS_PAUSED:
					___TargetAlpha_WarpPanel = 0.75f;
					___m_JumpButtonLabel.text = "Jump Prep Paused";
					___m_JumpButtonLabelTop.text = "Jump Prep Paused";
					break;
				case EWarpChargeStage.E_WCS_READY:
				{
					___TargetAlpha_WarpPanel = 0.3f;
					bool flag13 = __instance.MyScreenHubBase.OptionalShipInfo.TagID > -3;
					if (flag13)
					{
						___m_JumpButtonLabel.text = "Not Responding";
						___m_JumpButtonLabelTop.text = "Not Responding";
					}
					else
					{
						bool flag14 = num2 > 0;
						if (flag14)
						{
							___m_JumpButtonLabel.text = "Prep The " + str;
							___m_JumpButtonLabelTop.text = "Prep The " + str;
						}
						else
						{
							bool flag15 = num3 > 0;
							if (flag15)
							{
								___m_JumpButtonLabel.text = "No Fuel on the " + str2;
								___m_JumpButtonLabelTop.text = "No Fuel on the " + str2;
							}
							else
							{
								bool flag16 = PLServer.Instance.m_ShipCourseGoals.Count == 0;
								if (flag16)
								{
									___m_JumpButtonLabel.text = "Captain Has Not Set Course";
									___m_JumpButtonLabelTop.text = "Captain Has Not Set Course";
								}
								else
								{
									bool flag17 = __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID != -1 && num == 0;
									if (flag17)
									{
										___m_JumpButtonLabel.text = "Captain Warp To Sector " + __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID.ToString();
										___m_JumpButtonLabelTop.text = "Captain Warp To Sector " + __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID.ToString();
									}
									else
									{
										bool flag18 = PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) > 0.5f;
										if (flag18)
										{
											___m_JumpButtonLabel.text = "Not Responding";
											___m_JumpButtonLabelTop.text = "Not Responding";
										}
										else
										{
											___m_JumpButtonLabel.text = string.Concat(new object[]
											{
												"Align ",
												text,
												"to ",
												map.ID
											});
											___m_JumpButtonLabelTop.text = string.Concat(new object[]
											{
												"Align ",
												text,
												" to ",
                                                map.ID
                                            });
										}
									}
								}
							}
						}
					}
					break;
				}
				case EWarpChargeStage.E_WCS_ACTIVE:
				{
					___TargetAlpha_WarpPanel = 0.3f;
					bool flag19 = PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0;
					if (flag19)
					{
						___m_JumpButtonLabel.text = "Skip Warp (" + __instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString() + ")";
						___m_JumpButtonLabelTop.text = "Skip Warp (" + __instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString() + ")";
					}
					else
					{
						___m_JumpButtonLabel.text = "Jump In Progress (" + __instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString() + ")";
						___m_JumpButtonLabelTop.text = "Jump In Progress (" + __instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString() + ")";
					}
					break;
				}
				default:
					___TargetAlpha_WarpPanel = 0.3f;
					___m_JumpButtonLabel.text = "Initiate Jump Prep";
					___m_JumpButtonLabelTop.text = "Initiate Jump Prep";
					break;
				}
				for (int i = 0; i < PLGlobal.NumWarpChargeStages; i++)
				{
					___ChargeStage_Label[i].text = ___ChargeStage_Name[i];
					___ChargeStage_BarMask[i].clipOffset = new Vector2((__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeState_Levels[i] - 1f) * ___ChargeStage_BarMask[i].width, 0f);
				}
			}
		}
	}
}
