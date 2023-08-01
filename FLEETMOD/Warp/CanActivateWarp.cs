using HarmonyLib;
using System.Collections.Generic;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLWarpDriveScreen), "CanActivateWarpDrive")]
    internal class CanActivateWarp
    {
        public static bool Prefix(ref bool __result, PLWarpDriveScreen __instance)
        {
            if (!Variables.isrunningmod) return true;
            if (__instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip())
            {
                __result = __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID != -1 && __instance.MyScreenHubBase.OptionalShipInfo.NumberOfFuelCapsules > 0 && PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) < 0.5f;
                if (__result) __result = OthersCanActivateWarp(__instance.MyScreenHubBase.OptionalShipInfo.ShipID).Key;
                return false;
            }
            __result = __instance.MyScreenHubBase.OptionalShipInfo.NumberOfFuelCapsules > 0 && PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) < 0.5f;
            return false;
        }
        internal static KeyValuePair<bool, string> OthersCanActivateWarp(int shipId)
        {
            if (Variables.Fleet == null || Variables.Fleet.Count <= 1) return new KeyValuePair<bool, string>(true, "");
            // Gets the Targetted Sector, if no course is plotted, uses Admiral ships targetting
            int WarpTarget = -1;
            if (PLStarmap.Instance != null && PLStarmap.Instance.CurrentShipPath != null && PLStarmap.Instance.CurrentShipPath.Count > 1)
                WarpTarget = PLStarmap.Instance.CurrentShipPath[1].ID;
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
            foreach (int pLShipID in Variables.Fleet.Keys)
            {
                PLShipInfoBase plshipInfoBase = PLEncounterManager.Instance.GetShipFromID(pLShipID);
                if (!plshipInfoBase.GetIsPlayerShip() || plshipInfoBase == null || pLShipID == shipId) continue;
                if (plshipInfoBase.NumberOfFuelCapsules < 1) return new KeyValuePair<bool, string>(false, $"Refuel the {plshipInfoBase.ShipNameValue}");
                if (plshipInfoBase.WarpChargeStage != EWarpChargeStage.E_WCS_READY) return new KeyValuePair<bool, string>(false, $"Prep the {plshipInfoBase.ShipNameValue}");
                PLShipInfo pLShipInfo = (PLShipInfo)plshipInfoBase;
                if (pLShipInfo != null && (pLShipInfo.BlockingCombatTargetOnboard || pLShipInfo.HasVirusOfType(EVirusType.WARP_DISABLE))) return new KeyValuePair<bool, string>(false, $"{plshipInfoBase.ShipNameValue} is not responding");
                if (WarpTarget != -1 && plshipInfoBase.WarpTargetID != WarpTarget) return new KeyValuePair<bool, string> (false, $"Align the {plshipInfoBase.ShipNameValue}");
            }
            return new KeyValuePair<bool, string>(true, "");
        }
    }
}