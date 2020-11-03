using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000030 RID: 48
	[HarmonyPatch(typeof(PLServer), "NetworkToggleWarpCharge")]
	internal class NetworkToggleWarpCharge
	{
		// Token: 0x0600005E RID: 94 RVA: 0x000089C8 File Offset: 0x00006BC8
		public static bool Prefix(PLServer __instance, int inShipID, int inWarpCharge, PhotonMessageInfo pmi)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = !PLServer.GetPlayerForPhotonPlayer(pmi.sender).GetPlayerName(false).Contains("•");
				if (flag2)
				{
					PLServer.Instance.photonView.RPC("AddNotification", pmi.sender, new object[]
					{
						"Sorry, You cannot use the Jump Computer without Fleet Mod.",
						PLServer.GetPlayerForPhotonPlayer(pmi.sender).GetPlayerID(),
						PLServer.Instance.GetEstimatedServerMs() + 3000,
						true
					});
					result = false;
				}
				else
				{
					bool isDebugBuild = Debug.isDebugBuild;
					if (isDebugBuild)
					{
						Debug.Log("QDB: ------------------------------------------------------------------------------------------------");
					}
					PLShipInfoBase shipFromID = PLEncounterManager.Instance.GetShipFromID(inShipID);
					bool flag3 = shipFromID != null;
					if (flag3)
					{
						bool flag4 = PhotonNetwork.isMasterClient && pmi.sender != null && shipFromID.WarpChargeStage != (EWarpChargeStage)inWarpCharge && shipFromID != null && shipFromID.GetIsPlayerShip();
						if (flag4)
						{
							PLPlayer playerForPhotonPlayer = PLServer.GetPlayerForPhotonPlayer(pmi.sender);
							bool flag5 = playerForPhotonPlayer != null && playerForPhotonPlayer.TeamID == 0 && !playerForPhotonPlayer.IsBot;
							if (flag5)
							{
								PLPlayer cachedFriendlyPlayerOfClass = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0);
								bool flag6 = cachedFriendlyPlayerOfClass != null && playerForPhotonPlayer != cachedFriendlyPlayerOfClass;
								if (flag6)
								{
									bool flag7 = inWarpCharge != 1;
									if (flag7)
									{
										bool flag8 = inWarpCharge == 2;
										if (flag8)
										{
											PLServer.Instance.photonView.RPC("AddNotification", cachedFriendlyPlayerOfClass.GetPhotonPlayer(), new object[]
											{
												"[PL] has paused the jump prep",
												playerForPhotonPlayer.GetPlayerID(),
												PLServer.Instance.GetEstimatedServerMs() + 6000,
												true
											});
										}
									}
									else
									{
										PLServer.Instance.photonView.RPC("AddNotification", cachedFriendlyPlayerOfClass.GetPhotonPlayer(), new object[]
										{
											"[PL] has started jump prep",
											playerForPhotonPlayer.GetPlayerID(),
											PLServer.Instance.GetEstimatedServerMs() + 6000,
											true
										});
									}
								}
							}
						}
						shipFromID.WarpChargeStage = (EWarpChargeStage)inWarpCharge;
					}
					result = false;
				}
			}
			return result;
		}
	}
}
