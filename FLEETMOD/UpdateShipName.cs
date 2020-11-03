using System;
using HarmonyLib;
using PulsarPluginLoader;

namespace FLEETMOD
{
	// Token: 0x02000025 RID: 37
	[HarmonyPatch(typeof(PLEditGameSettingsMenu), "UpdateShipName")]
	internal class UpdateShipName
	{
		// Token: 0x06000048 RID: 72 RVA: 0x000067E8 File Offset: 0x000049E8
		public static bool Prefix(ref DBM_InputField ___ShipNameInput, ref string ___CachedShipName)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = ___ShipNameInput.Field != null && ___CachedShipName != ___ShipNameInput.Field.text;
				if (flag2)
				{
					PLEncounterManager.Instance.PlayerShip.photonView.RPC("Captain_NameShip", PhotonTargets.MasterClient, new object[]
					{
						___ShipNameInput.Field.text
					});
					ModMessage.SendRPC("Michael.Fleetmod", "FLEETMOD.ServerChangePlayerNames", PhotonTargets.MasterClient, new object[]
					{
						___ShipNameInput.Field.text,
						PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().GetScore()
					});
				}
				result = false;
			}
			return result;
		}
	}
}
