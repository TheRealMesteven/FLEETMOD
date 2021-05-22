using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLShipInfoBase), "AddHostileShip")]
    internal class AddHostileShip
    {// When a Crew ship spawns, it gets identified as a friendly rather than a hostile.
        public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase inShip, ref bool ___HostileShipAdded_NeedsResetForTargeting)
        {
            if (inShip != null && !__instance.HostileShips.Contains(inShip.ShipID) && !Global.GetIsFleetShip(__instance.ShipID))
            {
                __instance.HostileShips.Add(inShip.ShipID);
                ___HostileShipAdded_NeedsResetForTargeting = true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PLShipInfoBase), "GetIsPlayerShip")]
    internal class GetIsPlayerShipPatch
    {// When a Crew ship spawns, it gets identified as a friendly rather than a hostile.
        public static bool Prefix(PLShipInfoBase __instance, ref bool __result)
        {
            bool result;
            if (__instance.TagID < -3)
            ///PLEncounterManager.Instance.PlayerShip.ShipID == __instance.ShipID || Global.GetIsFleetShip(__instance.ShipID))
            /// Above if statement causes an error if refined to the above.
            {
                __result = true;
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
    [HarmonyPatch(typeof(PLShipInfo), "CheckForIntruders")]
    internal class IntruderAlertPatch
    {// When a Crew ship spawns, it gets identified as a friendly rather than a hostile.
        public static bool Prefix(PLShipInfo __instance, ref float ___LastIntruderAlarmStartedTime)
        {
            if (__instance.IsAuxSystemActive(6))
            {
                bool flag = false;
                foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                {
                    if (plplayer != null && plplayer.StartingShip != __instance && plplayer.PlayerLifeTime > 5f && plplayer.GetPawn() != null && plplayer.GetPawn().CurrentShip == __instance)
                    {
                        if (!(plplayer.TeamID == 0 && __instance.GetIsPlayerShip()))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    foreach (PLCombatTarget plcombatTarget in PLGameStatic.Instance.AllCombatTargets)
                    {
                        if (plcombatTarget != null && plcombatTarget.GetPlayer() == null && plcombatTarget.Lifetime > 4f && plcombatTarget.ShouldShowInHUD() && !plcombatTarget.GetIsFriendly() && plcombatTarget.CurrentShip == __instance)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    __instance.AlertLevel = 2;
                    ___LastIntruderAlarmStartedTime = Time.time;
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                    {
                    "INTRUDER ALARM!",
                    Color.red,
                    0,
                    "SHIP"
                    });
                    PLServer.Instance.photonView.RPC("ConsoleMessage", PhotonTargets.All, new object[]
                    {
                    "INTRUDER ALARM!"
                    });
                }
            }
            return false;
        }
    }
}