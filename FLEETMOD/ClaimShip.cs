using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200002E RID: 46
	[HarmonyPatch(typeof(PLServer), "ClaimShip")]
	internal class ClaimShip
	{
		// Token: 0x0600005A RID: 90 RVA: 0x000087E8 File Offset: 0x000069E8
		public static bool Prefix(PLServer __instance, int inShipID)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
				{
					bool flag2 = plplayer != null && plplayer.IsBot && plplayer.StartingShip == PLEncounterManager.Instance.GetShipFromID(inShipID);
					if (flag2)
					{
						plplayer.StartingShip = null;
					}
				}
				if (!PhotonNetwork.isMasterClient)
				{
					result = false;
				}
				else
				{
					if (PLEncounterManager.Instance.GetShipFromID(inShipID).TagID != -23 && PLEncounterManager.Instance.GetShipFromID(inShipID).TeamID == 1)
					{
						foreach (PLPlayer plplayer2 in PLServer.Instance.AllPlayers)
						{
							if (plplayer2 != null && !plplayer2.IsBot)
							{
								PLServer.Instance.photonView.RPC("AddNotification", plplayer2.GetPhotonPlayer(), new object[]
								{
									PLEncounterManager.Instance.GetShipFromID(inShipID).ShipNameValue + " Is Now A Neutral Ship.",
									plplayer2.GetPlayerID(),
									PLServer.Instance.GetEstimatedServerMs() + 3000,
									true
								});
							}
						}
						PLEncounterManager.Instance.GetShipFromID(inShipID).TeamID = -1;
						result = false;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}
	}
}
