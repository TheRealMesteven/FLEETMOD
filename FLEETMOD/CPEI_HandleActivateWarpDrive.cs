using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000031 RID: 49
	[HarmonyPatch(typeof(PLServer), "CPEI_HandleActivateWarpDrive")]
	internal class CPEI_HandleActivateWarpDrive
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00008C2C File Offset: 0x00006E2C
		public static bool Prefix(int shipID, int playerID)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = !PLServer.Instance.GetPlayerFromPlayerID(playerID).GetPlayerName(false).Contains("•");
				if (flag2)
				{
					result = false;
				}
				else
				{
					PLServer.Instance.photonView.RPC("AddNotification", PhotonTargets.All, new object[]
					{
						PLServer.Instance.GetPlayerFromPlayerID(playerID).GetPlayerName(false) + " has engaged the warp! Heading to: " + PLEncounterManager.Instance.GetShipFromID(shipID).WarpTargetID.ToString(),
						playerID,
						PLServer.Instance.GetEstimatedServerMs() + 3000,
						true
					});
					int num = UnityEngine.Random.Range(0, 10);
					bool flag3 = num == 50 && PLNetworkManager.Instance.LocalPlayer != null;
					if (flag3)
					{
						int num2 = -1;
						for (int i = 0; i < PLGlobal.Instance.Galaxy.AllSectorInfos.Count * 8; i++)
						{
							int num3 = UnityEngine.Random.Range(0, PLGlobal.Instance.Galaxy.AllSectorInfos.Count * 2);
							bool flag4 = PLGlobal.Instance.Galaxy.AllSectorInfos.ContainsKey(num3);
							if (flag4)
							{
								PLSectorInfo plsectorInfo = PLGlobal.Instance.Galaxy.AllSectorInfos[num3];
								bool flag5 = plsectorInfo != null && plsectorInfo.ID != PLServer.Instance.GetCurrentHubID();
								if (flag5)
								{
									num2 = num3;
									break;
								}
							}
						}
						bool flag6 = num2 != -1;
						if (flag6)
						{
							PLServer.Instance.photonView.RPC("NetworkBeginWarp", PhotonTargets.All, new object[]
							{
								PLEncounterManager.Instance.PlayerShip.ShipID,
								num2,
								PLServer.Instance.GetEstimatedServerMs(),
								-1
							});
						}
					}
					else
					{
						bool flag7 = PLNetworkManager.Instance.LocalPlayer != null;
						if (flag7)
						{
							PLServer.Instance.photonView.RPC("NetworkBeginWarp", PhotonTargets.All, new object[]
							{
								PLEncounterManager.Instance.PlayerShip.ShipID,
								PLEncounterManager.Instance.GetShipFromID(shipID).WarpTargetID,
								PLServer.Instance.GetEstimatedServerMs(),
								-1
							});
						}
					}
					result = false;
				}
			}
			return result;
		}
	}
}
