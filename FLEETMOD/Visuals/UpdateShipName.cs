using System;
using HarmonyLib;
using PulsarModLoader;

namespace FLEETMOD.Visuals
{
    [HarmonyPatch(typeof(PLEditGameSettingsMenu), "UpdateShipName")]
    internal class UpdateShipName
    {
        /// <summary>
        /// Updates Ship Names.
        /// Used to update player names, now is done automatically.
        /// </summary>
        public static bool Prefix(ref DBM_InputField ___ShipNameInput, ref string ___CachedShipName)
        {
            if (!MyVariables.isrunningmod) return true;
            if (___ShipNameInput.Field != null && ___CachedShipName != ___ShipNameInput.Field.text)
            {
                PLEncounterManager.Instance.PlayerShip.photonView.RPC("Captain_NameShip", PhotonTargets.MasterClient, new object[]
                {
                        ___ShipNameInput.Field.text
                });
                ModMessage.SendRPC(Plugin.harmonyIden, "FLEETMOD.ModMessages.ServerChangePlayerNames", PhotonTargets.MasterClient, new object[]
                {
                        ___ShipNameInput.Field.text,
                        PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().GetScore()
                });
            }
            return false;
        }
    }
}
