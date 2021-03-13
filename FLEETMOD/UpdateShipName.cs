using System;
using HarmonyLib;
using PulsarPluginLoader;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLEditGameSettingsMenu), "UpdateShipName")]
	internal class UpdateShipName
	{
		public static bool Prefix(ref DBM_InputField ___ShipNameInput, ref string ___CachedShipName)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (___ShipNameInput.Field != null && ___CachedShipName != ___ShipNameInput.Field.text)
				{
					PLEncounterManager.Instance.PlayerShip.photonView.RPC("Captain_NameShip", PhotonTargets.MasterClient, new object[]
					{
						___ShipNameInput.Field.text
					});
					ModMessage.SendRPC("Michael+Mest.Fleetmod", "FLEETMOD.ServerChangePlayerNames", PhotonTargets.MasterClient, new object[]
					{
						___ShipNameInput.Field.text,
						PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().GetScore()
					});
				}
				return false;
			}
		}
	}
}
