using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "RemovePlayer")]
	internal class RemovePlayer
	{
		public static bool Prefix(PLServer __instance, ref PLPlayer inPlayer)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (inPlayer != null)
				{
					PLServer.Instance.photonView.RPC("LogoutMessage", PhotonTargets.All, new object[]
					{
						inPlayer.GetPlayerName(false)
					});
					int num = 0;
					if (inPlayer.GetPhotonPlayer().GetScore() > 0 && inPlayer.StartingShip != null && !inPlayer.StartingShip.HasBeenDestroyed && inPlayer.GetClassID() == 0)
					{
						foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
						{
							if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer != inPlayer && plplayer.PlayerLifeTime > 10f && plplayer.GetPlayerName(false).Contains("•") && plplayer.GetClassID() != 0 && plplayer.GetPhotonPlayer().GetScore() == inPlayer.GetPhotonPlayer().GetScore() && !plplayer.IsBot)
							{
								num++;
								PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
								{
									plplayer.GetPlayerID(),
									0
								});
								PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
								{
									plplayer.GetPlayerName(false) + " \n is now the Captain",
									Color.green,
									0,
									"SHIP"
								});
								break;
							}
						}
					}
					/*if (PhotonNetwork.isMasterClient && num == 0 && inPlayer.StartingShip != PLNetworkManager.Instance.LocalPlayer.StartingShip && inPlayer.GetClassID() == 0)
					{
						inPlayer.StartingShip.DestroySelf(inPlayer.StartingShip);
						UnityEngine.Object.Destroy(inPlayer.StartingShip.gameObject);
					}*/
					PLServer.Instance.ClearPlayerData(inPlayer);
					PLServer.Instance.AllPlayers.Remove(inPlayer);
					MyVariables.survivalBonusDict.Remove(inPlayer.GetPlayerID());//Removing player from healthBonus dictonary on leave
					if (MyVariables.UnModdedCrews.ContainsKey(inPlayer.GetPlayerID()))
                    {
						MyVariables.UnModdedCrews.Remove(inPlayer.GetPlayerID());
					}
					if (inPlayer.GetPawn() != null)
					{
						inPlayer.GetPawn().transform.parent = null;
						inPlayer.GetPawn().gameObject.SetActive(true);
						PhotonNetwork.Destroy(inPlayer.GetPawn().gameObject);
					}
					PhotonNetwork.Destroy(inPlayer.gameObject);
				}
				return false;
			}
		}
	}
}
