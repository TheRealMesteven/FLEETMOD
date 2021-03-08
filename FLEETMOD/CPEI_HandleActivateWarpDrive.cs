using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "CPEI_HandleActivateWarpDrive")]
	internal class CPEI_HandleActivateWarpDrive
	{
		public static bool Prefix(int shipID, int playerID)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (!PLServer.Instance.GetPlayerFromPlayerID(playerID).GetPlayerName(false).Contains("•"))
				{
					return false;
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
					if (num == 50 && PLNetworkManager.Instance.LocalPlayer != null)
					{
						int num2 = -1;
						for (int i = 0; i < PLGlobal.Instance.Galaxy.AllSectorInfos.Count * 8; i++)
						{
							int num3 = UnityEngine.Random.Range(0, PLGlobal.Instance.Galaxy.AllSectorInfos.Count * 2);
							if (PLGlobal.Instance.Galaxy.AllSectorInfos.ContainsKey(num3))
							{
								PLSectorInfo plsectorInfo = PLGlobal.Instance.Galaxy.AllSectorInfos[num3];
								if (plsectorInfo != null && plsectorInfo.ID != PLServer.Instance.GetCurrentHubID())
								{
									num2 = num3;
									break;
								}
							}
						}
						if (num2 != -1)
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
						if (PLNetworkManager.Instance.LocalPlayer != null)
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
					return false;
				}
			}
		}
	}
}
