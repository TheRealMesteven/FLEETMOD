using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipInfoBase), "AboutToBeDestroyed")]
	internal class AboutToBeDestroyed
	{
		public static bool Prefix(PLShipInfoBase __instance)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (!__instance.HasBeenDestroyed && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
				{
					if (__instance.TagID == -23 && PhotonNetwork.isMasterClient)
					{
						PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
						{
							"The " + __instance.ShipNameValue + " Destroyed!",
							Color.green,
							0,
							"SHIP"
						});
					}
					if (!PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().GetScore() == __instance.ShipID)
					{
						PLUIEscapeMenu.Instance.OnClick_Disconnect();
						return false;
					}
					if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip == __instance as PLShipInfo)
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
					if (PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == __instance && PLNetworkManager.Instance.LocalPlayer.StartingShip != __instance as PLShipInfo)
					{
						PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
						{
							PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
							0
						});
					}
					__instance.TagID = -1;
				}
				return true;
			}
		}
	}
}
