using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLWarpDriveScreen), "Update")]
    internal class UpdatePLWarpDriveScreen
    {
        protected static MethodInfo CanActivateWarpDriveInfo = AccessTools.Method(typeof(PLWarpDriveScreen), "CanActivateWarpDrive");
        /// <summary>
        /// Change what the WarpDriveScreen parameters display
        /// (Self Destruct, Warp Button etc)
        /// </summary>
        public static void Postfix(PLWarpDriveScreen __instance, ref UISprite ___JumpComputerPanel, ref UISprite ___WarpDrivePanel, ref UISprite ___m_BlockingTargetOnboardPanel, ref UILabel ___m_JumpButtonLabel, ref UILabel ___BlindJumpBtnLabel, ref UILabel ___BlindJumpWarning, ref UISprite ___BlindJumpBtn, ref float ___TargetAlpha_WarpPanel, ref UILabel ___m_JumpButtonLabelTop, ref UILabel ___m_BlockingTargetOnboardPanelTitle, ref UIPanel ___m_JumpButtonMask, ref UIPanel[] ___ChargeStage_BarMask, ref UILabel[] ___ChargeStage_Label, string[] ___ChargeStage_Name)
        {
            if (!Variables.isrunningmod || __instance == null) return;

            // Gets the Targetted Sector, if no course is plotted, uses Admiral ships targetting
            int WarpTarget = -1;
            if (PLStarmap.Instance != null && PLStarmap.Instance.CurrentShipPath != null && PLStarmap.Instance.CurrentShipPath.Count > 1)
            {
                WarpTarget = PLStarmap.Instance.CurrentShipPath[1].ID;
            }
            else
            {
                PLShipInfoBase AdmiralShip = PLEncounterManager.Instance.GetShipFromID(PhotonNetwork.masterClient.GetScore());
                if (AdmiralShip != null)
                {
                    WarpTarget = AdmiralShip.WarpTargetID;
                }
            }

            // Default Warp Screen Config
            PLGlobal.SafeGameObjectSetActive(___WarpDrivePanel.gameObject, !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard && __instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage != EWarpChargeStage.E_WCS_ACTIVE && __instance.MyScreenHubBase.OptionalShipInfo.NumberOfFuelCapsules > 0 && !__instance.MyScreenHubBase.OptionalShipInfo.InWarp && !__instance.MyScreenHubBase.OptionalShipInfo.Abandoned);
            PLGlobal.SafeGameObjectSetActive(___JumpComputerPanel.gameObject, !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard);
            PLGlobal.SafeGameObjectSetActive(___m_BlockingTargetOnboardPanel.gameObject, __instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard);
            if (__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard)
            {
                Color b = Color.Lerp(new Color(0.8f, 0f, 0f, 1f), new Color(0.65f, 0.65f, 0.65f), Time.time % 1f);
                ___m_BlockingTargetOnboardPanelTitle.color = Color.Lerp(___m_BlockingTargetOnboardPanelTitle.color, b, Time.deltaTime * 2f);
            }
            if (__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard || __instance.MyScreenHubBase.OptionalShipInfo.HasVirusOfType(EVirusType.WARP_DISABLE) || PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) > 0.5f)
            {
                ___BlindJumpBtnLabel.text = "ERROR";
                ___BlindJumpBtnLabel.color = Color.black;
                ___BlindJumpWarning.text = "ERROR: Blind Jump Unavailable!";
                ___BlindJumpBtn.spriteName = "button_fill";
            }
            else
            {
                // Admiral Destruct Ship || Warp-Target Display
                if (PhotonNetwork.isMasterClient)
                {
                    if (__instance.MyScreenHubBase.OptionalShipInfo.BlindJumpUnlocked)
                    {
                        if (__instance.MyScreenHubBase.OptionalShipInfo.ShipID == PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID)
                        {
                            ___BlindJumpBtnLabel.text = "BLIND JUMP";
                            ___BlindJumpBtnLabel.color = Color.black;
                            ___BlindJumpWarning.text = "ADMIRAL - BLIND JUMP";
                            ___BlindJumpBtn.spriteName = "button_fill";
                        }
                        else
                        {
                            ___BlindJumpBtnLabel.text = "DESTROY SHIP";
                            ___BlindJumpBtnLabel.color = Color.black;
                            ___BlindJumpWarning.text = "For Emergency Use By The Admiral";
                            ___BlindJumpBtn.spriteName = "button_fill";
                        }
                    }
                    else
                    {
                        if (__instance.MyScreenHubBase.OptionalShipInfo.ShipID != PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID)
                        {
                            ___BlindJumpWarning.text = "ADMIRAL - DESTROY SHIP";
                            ___BlindJumpBtn.spriteName = "button";
                            ___BlindJumpBtnLabel.color = Color.red;
                            ___BlindJumpBtnLabel.text = "UNLOCK";
                        }
                        else
                        {
                            ___BlindJumpWarning.text = "ADMIRAL - BLIND JUMP";
                            ___BlindJumpBtn.spriteName = "button";
                            ___BlindJumpBtnLabel.color = Color.red;
                            ___BlindJumpBtnLabel.text = "UNLOCK";
                        }
                    }
                }
                else
                {
                    ___BlindJumpWarning.text = "Target Sector";
                    if (WarpTarget != -1)
                    {
                        ___BlindJumpBtnLabel.text = WarpTarget.ToString();
                    }
                    else
                    {
                        ___BlindJumpBtnLabel.text = "No Target Sector";
                    }
                }
            }

            // Warp Button Name Change based On Ship(s) Status(s)
            float warpChargePercentTotal = __instance.MyScreenHubBase.OptionalShipInfo.GetWarpChargePercentTotal();
            ___m_JumpButtonMask.clipOffset = new Vector2((warpChargePercentTotal - 1f) * ___m_JumpButtonMask.width, 0f);
            switch (__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage)
            {
                case EWarpChargeStage.E_WCS_PREPPING:
                    ___TargetAlpha_WarpPanel = 1f;
                    ___m_JumpButtonLabel.text = "Charging Warp Drive";
                    break;
                case EWarpChargeStage.E_WCS_PAUSED:
                    ___TargetAlpha_WarpPanel = 0.75f;
                    ___m_JumpButtonLabel.text = "Jump Prep Paused";
                    break;
                case EWarpChargeStage.E_WCS_READY:
                    {
                        ___TargetAlpha_WarpPanel = 0.3f;
                        KeyValuePair<bool, string> OtherShipStatus = CanActivateWarp.OthersCanActivateWarp(__instance.MyScreenHubBase.OptionalShipInfo.ShipID);
                        if (__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard || __instance.MyScreenHubBase.OptionalShipInfo.HasVirusOfType(EVirusType.WARP_DISABLE) || PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) > 0.5f)
                            ___m_JumpButtonLabel.text = "Not Responding";
                        else if (WarpTarget == -1) ___m_JumpButtonLabel.text = "No Target Sector";
                        else if (__instance.MyScreenHubBase.OptionalShipInfo.NumberOfFuelCapsules < 1) ___m_JumpButtonLabel.text = "No Fuel";
                        else if (__instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID != WarpTarget) ___m_JumpButtonLabel.text = $"Align to {WarpTarget}";
                        else if (!OtherShipStatus.Key) ___m_JumpButtonLabel.text = OtherShipStatus.Value;
                        else
                        {
                            PLSectorInfo sectorWithID = PLServer.GetSectorWithID(__instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID);
                            if (sectorWithID != null)
                            {
                                if (sectorWithID.VisualIndication == ESectorVisualIndication.COMET) ___m_JumpButtonLabel.text = "Jump to Comet";
                                else ___m_JumpButtonLabel.text = "Jump to " + sectorWithID.Name;
                            }
                            else ___m_JumpButtonLabel.text = "Jump to " + __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID.ToString();
                        }
                        break;
                    }
                case EWarpChargeStage.E_WCS_ACTIVE:
                    {
                        ___TargetAlpha_WarpPanel = 0.3f;
                        if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
                        {
                            ___m_JumpButtonLabel.text = $"Skip Warp ({__instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString()})";
                        }
                        else
                        {
                            ___m_JumpButtonLabel.text = $"Jump In Progress ({__instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString()})";
                        }
                        break;
                    }
                default:
                    ___TargetAlpha_WarpPanel = 0.3f;
                    ___m_JumpButtonLabel.text = "Initiate Jump Prep";
                    break;
            }
            ___m_JumpButtonLabelTop.text = ___m_JumpButtonLabel.text;
            for (int i = 0; i < PLGlobal.NumWarpChargeStages; i++)
            {
                ___ChargeStage_Label[i].text = ___ChargeStage_Name[i];
                ___ChargeStage_BarMask[i].clipOffset = new Vector2((__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeState_Levels[i] - 1f) * ___ChargeStage_BarMask[i].width, 0f);
            }
        }
    }
}
