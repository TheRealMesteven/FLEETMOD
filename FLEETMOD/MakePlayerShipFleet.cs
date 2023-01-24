using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLServer), "SpawnPlayerShip")]
    internal class MakePlayerShipFleet
    {
        public static void Postfix(PLServer __instance)
        {
            if (__instance != null && PhotonNetwork.isMasterClient)
            {
                MyVariables.Fleet.Add(PLEncounterManager.Instance.PlayerShip.ShipID, new List<int>());
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "SpawnPlayerShipFromSaveData")]
    internal class MakePlayerShipFleetSave
    {
        public static void Postfix(PLServer __instance)
        {
            if (__instance != null && PhotonNetwork.isMasterClient)
            {
                MyVariables.Fleet.Add(PLEncounterManager.Instance.PlayerShip.ShipID, new List<int>());
            }
        }
    }
}
