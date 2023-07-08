using ExitGames.Client.Photon;
using HarmonyLib;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(Room), "SetCustomProperties")]
    internal class CustomProperties
    {
        /// <summary>
        /// Replaces the room Hashtable with custom values.
        /// </summary>
        public static void Prefix(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
        {
            if (!Variables.isrunningmod || propertiesToSet == null || !PhotonNetwork.isMasterClient) return;
			if (PLEncounterManager.Instance.PlayerShip != null)
			{
                propertiesToSet["Ship_Name"] = PLEncounterManager.Instance.PlayerShip.ShipNameValue;
                propertiesToSet["Ship_Type"] = Mod.myversion;
			}
        }
    }
}