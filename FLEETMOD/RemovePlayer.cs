using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x0200002B RID: 43
	[HarmonyPatch(typeof(PLServer), "RemovePlayer")]
	internal class RemovePlayer
	{
		// Token: 0x06000054 RID: 84 RVA: 0x0000834C File Offset: 0x0000654C
		public static bool Prefix(PLServer __instance, ref PLPlayer inPlayer)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = inPlayer != null;
				if (flag2)
				{
					PLServer.Instance.photonView.RPC("LogoutMessage", PhotonTargets.All, new object[]
					{
						inPlayer.GetPlayerName(false)
					});
					int num = 0;
					bool flag3 = inPlayer.GetPhotonPlayer().GetScore() > 0 && inPlayer.StartingShip != null && !inPlayer.StartingShip.HasBeenDestroyed && inPlayer.GetClassID() == 0;
					if (flag3)
					{
						foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
						{
							bool flag4 = plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer != inPlayer && plplayer.PlayerLifeTime > 10f && plplayer.GetClassID() != 0 && plplayer.GetPhotonPlayer().GetScore() == inPlayer.GetPhotonPlayer().GetScore() && !plplayer.IsBot;
							if (flag4)
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
					bool flag5 = PhotonNetwork.isMasterClient && num == 0 && inPlayer.StartingShip != PLNetworkManager.Instance.LocalPlayer.StartingShip && inPlayer.GetClassID() == 0;
					if (flag5)
					{
						inPlayer.StartingShip.DestroySelf(inPlayer.StartingShip);
						UnityEngine.Object.Destroy(inPlayer.StartingShip.gameObject);
					}
					PLServer.Instance.ClearPlayerData(inPlayer);
					PLServer.Instance.AllPlayers.Remove(inPlayer);
					bool flag6 = inPlayer.GetPawn() != null;
					if (flag6)
					{
						inPlayer.GetPawn().transform.parent = null;
						inPlayer.GetPawn().gameObject.SetActive(true);
						PhotonNetwork.Destroy(inPlayer.GetPawn().gameObject);
					}
					PhotonNetwork.Destroy(inPlayer.gameObject);
				}
				result = false;
			}
			return result;
		}
	}
}
