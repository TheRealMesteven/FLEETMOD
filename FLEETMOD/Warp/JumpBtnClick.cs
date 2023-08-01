using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using HarmonyLib;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLWarpDriveScreen), "JumpBtnClick")]
    internal class JumpBtnClick
    {
        protected static MethodInfo CanActivateWarpDriveInfo = AccessTools.Method(typeof(PLWarpDriveScreen), "CanActivateWarpDrive");
        /// <summary>
        /// Patch to ensure Fleet cannot warp if Unaligned / Uncharged / Unfueled
        /// </summary>
        public static bool Prefix(PLWarpDriveScreen __instance)
        {
            if (!Variables.isrunningmod) return true;
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
                            if ((bool)CanActivateWarpDriveInfo.Invoke(__instance, null))
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
