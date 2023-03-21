using HarmonyLib;
using System.Collections.Generic;

namespace FLEETMOD.Ships
{
    [HarmonyPatch(typeof(PLServer), "SpawnPlayerShip")]
    internal class MakePlayerShipFleet
    {
        public static void Postfix(PLServer __instance)
        {
            if (__instance != null && PhotonNetwork.isMasterClient)
            {
                Variables.Fleet.Add(PLEncounterManager.Instance.PlayerShip.ShipID, new List<int>());
                PLEncounterManager.Instance.PlayerShip.AutoTarget = false;
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
                Variables.Fleet.Add(PLEncounterManager.Instance.PlayerShip.ShipID, new List<int>());
                PLEncounterManager.Instance.PlayerShip.AutoTarget = false;
            }
        }
    }
}
