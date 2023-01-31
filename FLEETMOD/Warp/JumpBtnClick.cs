using System;
using System.Runtime.Remoting.Messaging;
using HarmonyLib;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLWarpDriveScreen), "JumpBtnClick")]
    internal class JumpBtnClick
    {
        /// <summary>
        /// Patch to ensure Fleet cannot warp if Unaligned / Uncharged / Unfueled
        /// </summary>
        public static bool Prefix(PLWarpDriveScreen __instance)
        {
            if (!MyVariables.isrunningmod) return true;

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

            // Get the status of each Fleetship, indicating if they can warp or not.
            // (In future, could be improved to allow ship warping if all targets aligned to the same location)
            bool CantWarp = false;
            foreach (int pLShipID in MyVariables.Fleet.Keys)
            {
                PLShipInfoBase plshipInfoBase = PLEncounterManager.Instance.GetShipFromID(pLShipID);
                if (plshipInfoBase.GetIsPlayerShip() && plshipInfoBase != null && (
                WarpTarget == -1 || plshipInfoBase.WarpTargetID != WarpTarget || plshipInfoBase.WarpChargeStage != EWarpChargeStage.E_WCS_READY || plshipInfoBase.NumberOfFuelCapsules < 1 
                || ((PLShipInfo)plshipInfoBase != null && (((PLShipInfo)plshipInfoBase).BlockingCombatTargetOnboard || ((PLShipInfo)plshipInfoBase).HasVirusOfType(EVirusType.WARP_DISABLE)))
                ))
                {
                    CantWarp = true;
                    break;
                }
            }

            // Detect changes in the screen and sync to everyone
            if (__instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip())
            {
                bool ChangeState = false;
                EWarpChargeStage ewarpChargeStage = EWarpChargeStage.E_WCS_COLD_START;
                switch (__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage)
                {
                    case EWarpChargeStage.E_WCS_PREPPING:
                        ewarpChargeStage = EWarpChargeStage.E_WCS_PAUSED;
                        ChangeState = true;
                        break;
                    case EWarpChargeStage.E_WCS_PAUSED:
                        ewarpChargeStage = EWarpChargeStage.E_WCS_PREPPING;
                        ChangeState = true;
                        break;
                    case EWarpChargeStage.E_WCS_READY:
                        {
                            if (!CantWarp && __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID != -1
                                && PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, true) < 0.5f)
                            {
                                ChangeState = true;
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
                                PLEncounterManager.Instance.PlayerShip.photonView.RPC("SkipWarp", PhotonTargets.All);
                            }
                            break;
                        }
                    default:
                        ewarpChargeStage = EWarpChargeStage.E_WCS_PREPPING;
                        ChangeState = true;
                        break;
                }
                if (ChangeState)
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
            return false;
        }
    }
}
