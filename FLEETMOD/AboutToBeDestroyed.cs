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
                    ///<summary>
                    /// The below lines of code sets each player of a destroyed ship as crew of the host ship.
                    ///</summary>
					if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip != __instance as PLShipInfo)
					{
						foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
						{
							if (plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot && plplayer.StartingShip == __instance)
							{
                                plplayer.SetClassID(1);
								plplayer.GetPhotonPlayer().SetScore(PhotonNetwork.player.GetScore());
								plplayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
							}
						}
					}
                    ///<summary>
                    /// Below lines of code sets each players ship as the host ship when the hosts ship is destroyed, causing the game to fail like normal.
                    ///</summary>
                    if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip == __instance as PLShipInfo)
                    {
                        foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                        {
                            if (plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot)
                            {
                                plplayer.GetPhotonPlayer().SetScore(PhotonNetwork.player.GetScore());
                                plplayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
                            }
                        }
                    }
                    ///
                    if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == __instance && PLNetworkManager.Instance.LocalPlayer.StartingShip != __instance as PLShipInfo)
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
 