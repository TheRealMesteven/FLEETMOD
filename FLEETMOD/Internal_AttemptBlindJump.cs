using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000032 RID: 50
	[HarmonyPatch(typeof(PLServer), "Internal_AttemptBlindJump")]
	internal class Internal_AttemptBlindJump
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00008EC0 File Offset: 0x000070C0
		public static bool Prefix(int inShipID)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID != inShipID && PhotonNetwork.isMasterClient;
				if (flag2)
				{
					PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
					{
						PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
						0
					});
					PLEncounterManager.Instance.GetShipFromID(inShipID).DestroySelf(PLEncounterManager.Instance.GetShipFromID(inShipID));
					UnityEngine.Object.Destroy(PLEncounterManager.Instance.GetShipFromID(inShipID).gameObject);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}
	}
}
