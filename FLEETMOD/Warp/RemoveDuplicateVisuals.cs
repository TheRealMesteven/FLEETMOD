using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLServer), "Internal_NetworkBeginWarp")]
    internal class RemoveDuplicateWarpVisuals
    {
        public static void Prefix(PLServer __instance, int inShipID, int HubID, int inBeginWarpServerTime, int playerIDStartedWarp, bool canGivePickupMissions)
        {
            if (!Variables.isrunningmod) return;
            foreach (int plshipID in Variables.Fleet.Keys)
            {
                PLShipInfoBase plshipInfoBase = PLEncounterManager.Instance.GetShipFromID(plshipID);
                if (plshipInfoBase != null) RemoveDuplicateWarps(plshipInfoBase);
            }
        }
        private static void RemoveDuplicateWarps(PLShipInfoBase pLShipInfoBase)
        {
            Transform WarpClone = pLShipInfoBase.Exterior.transform.Find("Warp(Clone)");
            if (PhotonNetwork.masterClient.GetScore() == pLShipInfoBase.ShipID) WarpClone.localScale = new Vector3(900, 900, 900);
            else
            {
                WarpClone.localScale = new Vector3(0, 0, 0);
            }
        }
    }
}
