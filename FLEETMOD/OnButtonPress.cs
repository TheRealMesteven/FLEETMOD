using System;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x0200000E RID: 14
	[HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "OnButtonPress")]
	internal class OnButtonPress
	{
		// Token: 0x06000017 RID: 23 RVA: 0x000031D4 File Offset: 0x000013D4
		public static bool Prefix(PLOverviewPlayerInfoDisplay __instance, ref PLTabMenuPlayerInfoButton inButton, ref float ___LastButtonPressProcessTime, ref float ___cached_LastUpdatedPlayerInfoTime)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = Time.time - ___LastButtonPressProcessTime < 0.1f;
				if (flag2)
				{
					result = false;
				}
				else
				{
					___LastButtonPressProcessTime = Time.time;
					bool flag3 = !PLNetworkManager.IsActiveMenuOpen() && PLTabMenu.Instance.TabMenuActive && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked";
					if (flag3)
					{
						bool flag4 = __instance.MyPlayer == null && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLNetworkManager.Instance.LocalPlayer.GetClassID() != 0 && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer() != null && PLNetworkManager.Instance.LocalPlayer.ArenaScore > 0 && PLServer.Instance != null;
						if (flag4)
						{
							bool flag5 = inButton.m_Label.text == "Pilot";
							if (flag5)
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
							bool flag6 = inButton.m_Label.text == "Science";
							if (flag6)
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
							bool flag7 = inButton.m_Label.text == "Weapons";
							if (flag7)
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
							bool flag8 = inButton.m_Label.text == "Engineer";
							if (flag8)
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
							bool flag9 = inButton.m_Label.text == "Remove Bot";
							if (flag9)
							{
								PLServer.Instance.photonView.RPC("ServerRemoveCrewBotPlayer", PhotonTargets.MasterClient, new object[]
								{
									__instance.MyPlayer.GetPlayerID()
								});
							}
							bool flag10 = inButton.m_Label.text == "Kick";
							if (flag10)
							{
								PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLKickPlayerMenu(__instance.MyPlayer));
							}
							bool flag11 = __instance.MyPlayer.GetPhotonPlayer() != null && __instance.MyPlayer.GetPhotonPlayer().SteamID != CSteamID.Nil;
							if (flag11)
							{
								bool flag12 = inButton.m_Label.text == "Add Friend";
								if (flag12)
								{
									SteamFriends.ActivateGameOverlayToUser("friendadd", __instance.MyPlayer.GetPhotonPlayer().SteamID);
								}
								else
								{
									bool flag13 = inButton.m_Label.text == "View Profile";
									if (flag13)
									{
										SteamFriends.ActivateGameOverlayToUser("steamid", __instance.MyPlayer.GetPhotonPlayer().SteamID);
									}
								}
							}
							bool isDebugBuild = Debug.isDebugBuild;
							if (isDebugBuild)
							{
								Debug.Log("-------------- QDB: CHANGING MUTE FUNCTION FOR MC ---------------");
							}
							bool isMasterClient = PhotonNetwork.isMasterClient;
							if (isMasterClient)
							{
								bool flag14 = inButton.m_Label.text == "To Brig";
								if (flag14)
								{
									int num = __instance.MyPlayer.GetPlayerName(false).LastIndexOf("•");
									___cached_LastUpdatedPlayerInfoTime = 0f;
									__instance.MyPlayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
									__instance.MyPlayer.GetPhotonPlayer().SetScore(PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID);
									PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
									{
										__instance.MyPlayer.GetPlayerName(false).Substring(num + 2) + " Has Been Sent To The Brig By The Admiral.",
										Color.green,
										0,
										"SHIP"
									});
									PLNetworkManager.Instance.ConsoleText.Insert(0, "You Have Sent " + __instance.MyPlayer.GetPlayerName(false).Substring(num + 2) + " To The Brig");
									__instance.MyPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
									{
										PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
										0
									});
									__instance.MyPlayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
									{
										"BRIG " + __instance.MyPlayer.GetPlayerName(false).Substring(num)
									});
								}
								bool flag15 = inButton.m_Label.text == "Release";
								if (flag15)
								{
									int num2 = __instance.MyPlayer.GetPlayerName(false).LastIndexOf("•");
									__instance.MyPlayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
									{
										"FREE " + __instance.MyPlayer.GetPlayerName(false).Substring(num2)
									});
									___cached_LastUpdatedPlayerInfoTime = 0f;
									PLNetworkManager.Instance.ConsoleText.Insert(0, "You Have Released " + __instance.MyPlayer.GetPlayerName(false).Substring(num2 + 2) + " From The Brig");
								}
							}
							bool flag16 = inButton.m_Label.text == "Mute";
							if (flag16)
							{
								__instance.MyPlayer.TS_IsMuted = true;
								___cached_LastUpdatedPlayerInfoTime = 0f;
								PLVoiceChatManager.Instance.MuteClient(__instance.MyPlayer.TS_ClientID);
							}
							bool flag17 = inButton.m_Label.text == "Unmute";
							if (flag17)
							{
								__instance.MyPlayer.TS_IsMuted = false;
								___cached_LastUpdatedPlayerInfoTime = 0f;
								PLVoiceChatManager.Instance.UnmuteClient(__instance.MyPlayer.TS_ClientID);
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
