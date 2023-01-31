using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLShipInfo), "CheckForIntruders")]
    internal class IntruderAlarm
    {
        // Overrides the default CheckForIntruders with additional checks for intruders being from FleetShip
        // (Only MasterClient calls this)
        protected static FieldInfo FieldInfo = AccessTools.Field(typeof(PLShipInfo), "LastIntruderAlarmStartedTime");
        public static bool Prefix(PLShipInfo __instance)
        {
            if (!MyVariables.isrunningmod || !MyVariables.Fleet.ContainsKey(__instance.ShipID)) return true;
            if (__instance.IsAuxSystemActive(6))
            {
                bool TriggerAlarm = false;
                foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                {
                    if (plplayer != null && plplayer.StartingShip != null && plplayer.StartingShip != __instance && !plplayer.StartingShip.GetIsPlayerShip() && plplayer.PlayerLifeTime > 5f && plplayer.GetPawn() != null && plplayer.GetPawn().CurrentShip == __instance)
                    {
                        TriggerAlarm = true;
                        break;
                    }
                }
                if (!TriggerAlarm)
                {
                    foreach (PLCombatTarget plcombatTarget in PLGameStatic.Instance.AllCombatTargets)
                    {
                        if (plcombatTarget != null && plcombatTarget.GetPlayer() == null && plcombatTarget.Lifetime > 4f && plcombatTarget.ShouldShowInHUD() && !plcombatTarget.GetIsFriendly() && plcombatTarget.CurrentShip == __instance)
                        {
                            TriggerAlarm = true;
                            break;
                        }
                    }
                }
                if (TriggerAlarm)
                {
                    __instance.AlertLevel = 2;
                    FieldInfo.SetValue(__instance, Time.time);
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                    {
                        $"{__instance.ShipNameValue.ToUpper()} INTRUDER ALARM!",
                        Color.red,
                        0,
                        "SHIP"
                    });
                    PLServer.Instance.photonView.RPC("ConsoleMessage", PhotonTargets.All, new object[]
                    {
                        $"{__instance.ShipNameValue.ToUpper()} INTRUDER ALARM!"
                    });
                }
            }
            return false;
        }
    }
}
