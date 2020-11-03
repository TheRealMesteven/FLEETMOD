using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000036 RID: 54
	[HarmonyPatch(typeof(PLShipInfoBase), "AboutToBeDestroyed")]
	internal class AboutToBeDestroyed
	{
		// Token: 0x0600006A RID: 106 RVA: 0x00009124 File Offset: 0x00007324
		public static bool Prefix(PLShipInfoBase __instance)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = !__instance.HasBeenDestroyed && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted();
				if (flag2)
				{
					bool flag3 = __instance.TagID == -23 && PhotonNetwork.isMasterClient;
					if (flag3)
					{
						PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
						{
							"The " + __instance.ShipNameValue + " Destroyed!",
							Color.green,
							0,
							"SHIP"
						});
					}
					bool flag4 = !PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().GetScore() == __instance.ShipID;
					if (flag4)
					{
						PLUIEscapeMenu.Instance.OnClick_Disconnect();
						return false;
					}
					bool flag5 = PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip == __instance as PLShipInfo;
					if (flag5)
					{
						foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
						{
							bool flag6 = plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot;
							if (flag6)
							{
								plplayer.GetPhotonPlayer().SetScore(PhotonNetwork.player.GetScore());
								plplayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
							}
						}
					}
					bool flag7 = PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == __instance && PLNetworkManager.Instance.LocalPlayer.StartingShip != __instance as PLShipInfo;
					if (flag7)
					{
						PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
						{
							PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
							0
						});
					}
					__instance.TagID = -1;
				}
				result = true;
			}
			return result;
		}
	}
}
