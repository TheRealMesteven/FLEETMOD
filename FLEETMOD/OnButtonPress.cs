using System;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "OnButtonPress")]
	internal class OnButtonPress
	{
		public static bool Prefix(PLOverviewPlayerInfoDisplay __instance, ref PLTabMenuPlayerInfoButton inButton, ref float ___LastButtonPressProcessTime, ref float ___cached_LastUpdatedPlayerInfoTime)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				if (Time.time - ___LastButtonPressProcessTime < 0.1f)
				{
					result = false;
				}
				else
				{
					___LastButtonPressProcessTime = Time.time;
					if (!PLNetworkManager.IsActiveMenuOpen() && PLTabMenu.Instance.TabMenuActive && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked")
					{
						if (__instance.MyPlayer == null && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLNetworkManager.Instance.LocalPlayer.GetClassID() != 0 && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer() != null && PLNetworkManager.Instance.LocalPlayer.ArenaScore > 0 && PLServer.Instance != null)
						{
							if (inButton.m_Label.text == "Pilot")
							{
								PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
								{
									PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
									1
								});
								PhotonNetwork.player.SetScore(PLNetworkManager.Instance.LocalPlayer.ArenaScore);
								PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
								PLTabMenu.Instance.TabMenuActive = false;
								PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
								{
									"<color=#FFFFFF>You Are Now Pilot Aboard The ",
									PLEncounterManager.Instance.GetShipFromID(PLNetworkManager.Instance.LocalPlayer.ArenaScore).ShipNameValue,
									"</color>\n\n<color=#c0c0c0>",
									PLGlobal.Instance.ClassDesc[1],
									"</color>"
								})));
							}
							if (inButton.m_Label.text == "Science")
							{
								PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
								{
									PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
									2
								});
								PhotonNetwork.player.SetScore(PLNetworkManager.Instance.LocalPlayer.ArenaScore);
								PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
								PLTabMenu.Instance.TabMenuActive = false;
								PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
								{
									"<color=#00FF00>You Are Now Scientist Aboard The ",
									PLEncounterManager.Instance.GetShipFromID(PLNetworkManager.Instance.LocalPlayer.ArenaScore).ShipNameValue,
									"</color>\n\n<color=#c0c0c0>",
									PLGlobal.Instance.ClassDesc[2],
									"</color>"
								})));
							}
							if (inButton.m_Label.text == "Weapons")
							{
								PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
								{
									PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
									3
								});
								PhotonNetwork.player.SetScore(PLNetworkManager.Instance.LocalPlayer.ArenaScore);
								PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
								PLTabMenu.Instance.TabMenuActive = false;
								PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
								{
									"<color=#FF0000>You Are Now Weapons Specialist Aboard The ",
									PLEncounterManager.Instance.GetShipFromID(PLNetworkManager.Instance.LocalPlayer.ArenaScore).ShipNameValue,
									"</color>\n\n<color=#c0c0c0>",
									PLGlobal.Instance.ClassDesc[3],
									"</color>"
								})));
							}
							if (inButton.m_Label.text == "Engineer")
							{
								PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
								{
									PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
									4
								});
								PhotonNetwork.player.SetScore(PLNetworkManager.Instance.LocalPlayer.ArenaScore);
								PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
								PLTabMenu.Instance.TabMenuActive = false;
								PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
								{
									"<color=#FF6600>You Are Now Engineer Aboard The ",
									PLEncounterManager.Instance.GetShipFromID(PLNetworkManager.Instance.LocalPlayer.ArenaScore).ShipNameValue,
									"</color>\n\n<color=#c0c0c0>",
									PLGlobal.Instance.ClassDesc[4],
									"</color>"
								})));
								return false;
							}
						}
						else
						{
							if (inButton.m_Label.text == "Remove Bot")
							{
								PLServer.Instance.photonView.RPC("ServerRemoveCrewBotPlayer", PhotonTargets.MasterClient, new object[]
								{
									__instance.MyPlayer.GetPlayerID()
								});
							}
							if (inButton.m_Label.text == "Kick")
							{
								PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLKickPlayerMenu(__instance.MyPlayer));
							}
							if (__instance.MyPlayer.GetPhotonPlayer() != null && __instance.MyPlayer.GetPhotonPlayer().SteamID != CSteamID.Nil)
							{
								if (inButton.m_Label.text == "Add Friend")
								{
									SteamFriends.ActivateGameOverlayToUser("friendadd", __instance.MyPlayer.GetPhotonPlayer().SteamID);
								}
								else
								{
									if (inButton.m_Label.text == "View Profile")
									{
										SteamFriends.ActivateGameOverlayToUser("steamid", __instance.MyPlayer.GetPhotonPlayer().SteamID);
									}
								}
							}
							if (Debug.isDebugBuild)
							{
								Debug.Log("-------------- QDB: CHANGING MUTE FUNCTION FOR MC ---------------");
							}
							if (PhotonNetwork.isMasterClient)
							{
								if (inButton.m_Label.text == "To Brig")
								{
									___cached_LastUpdatedPlayerInfoTime = 0f;
									__instance.MyPlayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
									__instance.MyPlayer.GetPhotonPlayer().SetScore(PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID);
									PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
									{
										__instance.MyPlayer.GetPlayerName(false).Substring(__instance.MyPlayer.GetPlayerName(false).LastIndexOf("•") + 2) + " Has Been Sent To The Brig By The Admiral.",
										Color.green,
										0,
										"SHIP"
									});
									PLNetworkManager.Instance.ConsoleText.Insert(0, "You Have Sent " + __instance.MyPlayer.GetPlayerName(false).Substring(__instance.MyPlayer.GetPlayerName(false).LastIndexOf("•") + 2) + " To The Brig");
									__instance.MyPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
									{
										PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
										0
									});
									__instance.MyPlayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
									{
										"BRIG " + __instance.MyPlayer.GetPlayerName(false).Substring(__instance.MyPlayer.GetPlayerName(false).LastIndexOf("•"))
									});
								}
								if (inButton.m_Label.text == "Release")
								{
									__instance.MyPlayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
									{
										"FREE " + __instance.MyPlayer.GetPlayerName(false).Substring(__instance.MyPlayer.GetPlayerName(false).LastIndexOf("•"))
									});
									___cached_LastUpdatedPlayerInfoTime = 0f;
									PLNetworkManager.Instance.ConsoleText.Insert(0, "You Have Released " + __instance.MyPlayer.GetPlayerName(false).Substring(__instance.MyPlayer.GetPlayerName(false).LastIndexOf("•") + 2) + " From The Brig");
								}
							}
							if (inButton.m_Label.text == "Mute")
							{
								__instance.MyPlayer.TS_IsMuted = true;
								___cached_LastUpdatedPlayerInfoTime = 0f;
								//PLVoiceChatManager.Instance.MuteClient(__instance.MyPlayer.TS_ClientID);
							}
							if (inButton.m_Label.text == "Unmute")
							{
								__instance.MyPlayer.TS_IsMuted = false;
								___cached_LastUpdatedPlayerInfoTime = 0f;
								//PLVoiceChatManager.Instance.UnmuteClient(__instance.MyPlayer.TS_ClientID);
							}
						}
					}
					result = false;
				}
			}
			return result;
		}
	}
}
