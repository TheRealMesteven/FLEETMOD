using System;
using ExitGames.Client.Photon;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000029 RID: 41
	[HarmonyPatch(typeof(PLServer), "Update")]
	internal class Update
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00006C74 File Offset: 0x00004E74
		public static void Postfix(PLServer __instance)
		{
			bool isrunningmod = MyVariables.isrunningmod;
            if (isrunningmod)
			{
				bool flag = __instance != null && __instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLEncounterManager.Instance.PlayerShip != null;
				if (flag)
				{
					PLEncounterManager.Instance.PlayerShip.TagID = -23;
					string myversion = Plugin.myversion;
					PLInGameUI.Instance.CurrentOrdersLabel.enabled = true;
					PLInGameUI.Instance.CurrentOrdersLabel.resizeTextForBestFit = true;
					PLInGameUI.Instance.CurrentOrdersLabel.supportRichText = true;
					PLInGameUI.Instance.CurrentOrdersLabel.text = "<color=#ffffff>Your Admiral </color><color=#ffff00>\n" + PLServer.Instance.GetPlayerFromPlayerID(0).GetPlayerName(true) + "</color>";
					PLInGameUI.Instance.CurrentVersionLabel.text = myversion;
					PLInGameUI.Instance.ControlsText.enabled = true;
					string str = "<color=#FFFFFF>";
					bool flag2 = PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0 && !PhotonNetwork.isMasterClient;
					if (flag2)
					{
						str = "<color=#0066FF>";
					}
					bool flag3 = PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0 && PhotonNetwork.isMasterClient;
					if (flag3)
					{
						str = "<color=#ffff00>";
					}
					bool flag4 = PLNetworkManager.Instance.LocalPlayer.GetClassID() == 1;
					if (flag4)
					{
						str = "<color=#FFFFFF>";
					}
					bool flag5 = PLNetworkManager.Instance.LocalPlayer.GetClassID() == 2;
					if (flag5)
					{
						str = "<color=#00FF00>";
					}
					bool flag6 = PLNetworkManager.Instance.LocalPlayer.GetClassID() == 3;
					if (flag6)
					{
						str = "<color=#FF0000>";
					}
					bool flag7 = PLNetworkManager.Instance.LocalPlayer.GetClassID() == 4;
					if (flag7)
					{
						str = "<color=#FF6600>";
					}
					PLInGameUI.Instance.ControlsText.text = string.Concat(new object[]
					{
						"Server : " + PhotonNetwork.room.Name + "\n \n",
						str + PLNetworkManager.Instance.LocalPlayer.GetPlayerName(true) + "\n \n </color>",
						string.Concat(new object[]
						{
							"Players : ",
							PhotonNetwork.room.PlayerCount,
							" / ",
							PhotonNetwork.room.MaxPlayers,
							"\n\n"
						})
					});
					bool flag8 = PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && !PLTabMenu.Instance.TabMenuActive && PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp != 0;
					if (flag8)
					{
						PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp = 0;
					}
					bool flag9 = PLStarmap.Instance.CurrentShipPath.Count > 2;
					if (flag9)
					{
						PLStarmap.Instance.CurrentShipPath.Clear();
						PLServer.Instance.m_ShipCourseGoals.Clear();
					}
					bool flag10 = __instance != null &&  PLNetworkManager.Instance.LocalPlayer.StartingShip != null && PLEncounterManager.Instance.PlayerShip != null;
					if (flag10)
					{
						bool flag11 = PLNetworkManager.Instance.LocalPlayer.StartingShip != null && PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip != null && PLNetworkManager.Instance.LocalPlayer.StartingShip.WarpTravelDist != PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.WarpTravelDist;
						if (flag11)
						{
							PLNetworkManager.Instance.LocalPlayer.StartingShip.WarpTravelDist = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.WarpTravelDist;
						}
						PLNetworkManager.Instance.LocalPlayer.StartingShip.LastBeginWarpServerTime = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.LastBeginWarpServerTime;
						bool flag12 = !PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F1) && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked";
						if (flag12)
						{
							PLMusic.PostEvent("play_sx_playermenu_click_major", PLServer.Instance.gameObject);
							PLNetworkManager.Instance.MainMenu.CloseActiveMenu();
							PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLCreateGameMenu(true));
							PLTabMenu.Instance.TabMenuActive = false;
						}
						bool flag13 = PLNetworkManager.Instance.LocalPlayer.StartingShip != null && PLNetworkManager.Instance.LocalPlayer.StartingShip.TeamID != 0;
						if (flag13)
						{
							PLNetworkManager.Instance.LocalPlayer.StartingShip.TeamID = 0;
						}
						bool flag14 = PLNetworkManager.Instance.LocalPlayer.GetPawn() != null && PLNetworkManager.Instance.LocalPlayer.GetPawn().Lifetime > 2f && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip != null;
						if (flag14)
						{
							bool flag15 = PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.GetIsPlayerShip() && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.TeamID != -1 && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip != PLNetworkManager.Instance.LocalPlayer.StartingShip;
							if (flag15)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.TeamID = -1;
							}
						}
						bool autoTarget = PLEncounterManager.Instance.PlayerShip.AutoTarget;
						if (autoTarget)
						{
							PLEncounterManager.Instance.PlayerShip.AutoTarget = false;
						}
						bool flag16 = (PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("BRIG") && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked" && PLNetworkManager.Instance.LocalPlayer.GetPawn().Lifetime > 1f && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip) || (!PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("BRIG") && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName == "locked" && !PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("FREE") && PLNetworkManager.Instance.LocalPlayer.GetPawn().Lifetime > 5f && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip);
						if (flag16)
						{
							bool flag17 = !PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("BRIG");
							if (flag17)
							{
								int startIndex = PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).LastIndexOf("•");
								PLNetworkManager.Instance.LocalPlayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
								{
									"BRIG " + PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Substring(startIndex)
								});
							}
							PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
							{
								"<color=#c0c0c0><size=15>You Have Been Confined To The Brig By The Admiral.\n\n Please Wait Here Until The Admiral Releases You.</size></color>"
							})));
							bool flag18 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_OUTRIDER;
							if (flag18)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(12.5f, -406f, -2f);
							}
							bool flag19 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_INTREPID;
							if (flag19)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(9.4f, -400.9f, -13.8f);
							}
							bool flag20 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_ROLAND;
							if (flag20)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(8.8f, -391f, -30.8f);
							}
							bool flag21 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.OLDWARS_HUMAN;
							if (flag21)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(-14.8f, -365.8f, 38.1f);
							}
							bool flag22 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_WDCRUISER;
							if (flag22)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(-9.4f, -397f, -5f);
							}
							bool flag23 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_DESTROYER;
							if (flag23)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(0.2f, 405f, 14.1f);
							}
							bool flag24 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_ANNIHILATOR;
							if (flag24)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(-5.6f, -400f, -1.9f);
							}
							bool flag25 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_CIVILIAN_STARTING_SHIP;
							if (flag25)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(-5.6f, -384.5f, 24f);
							}
							bool flag26 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_STARGAZER;
							if (flag26)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(-7.3f, -394.5f, 13.6f);
							}
							bool flag27 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_CARRIER;
							if (flag27)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(1.2f, -386f, 18.4f);
							}
							bool flag28 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.OLDWARS_SYLVASSI;
							if (flag28)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(-16.1f, -397.5f, -15.5f);
							}
							bool flag29 = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID == EShipType.E_INTREPID_SC;
							if (flag29)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().transform.position = new Vector3(9.4f, -400.9f, -13.8f);
							}
							PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName = "locked";
							foreach (PLDoor pldoor in PLDoor.AllDoors)
							{
								bool flag30 = pldoor != null;
								if (flag30)
								{
									pldoor.OpenRange = 0.5f;
								}
							}
						}
						bool flag31 = PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("FREE") && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "open";
						if (flag31)
						{
							PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName = "open";
							PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
							{
								"<color=#c0c0c0><size=15>You Have Been Released From The Brig.</size></color>"
							})));
							foreach (PLDoor pldoor2 in PLDoor.AllDoors)
							{
								bool flag32 = pldoor2 != null;
								if (flag32)
								{
									pldoor2.OpenRange = 2.5f;
								}
							}
						}
					}
					bool flag33 = PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLServer.Instance.GameHasStarted;
					if (flag33)
					{
						bool flag34 = PLNetworkManager.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PhotonNetwork.isMasterClient && PLServer.Instance != null && PLServer.Instance.AllPlayersLoaded() && PLEncounterManager.Instance.PlayerShip != null && Mathf.Abs((float)((long)PLServer.Instance.GetEstimatedServerMs() - (long)PLEncounterManager.Instance.PlayerShip.LastBeginWarpServerTime)) > 16000f && PhotonNetwork.player.NickName != "skipwarp";
						if (flag34)
						{
							PhotonNetwork.player.NickName = "skipwarp";
						}
						bool flag35 = !PLEncounterManager.Instance.PlayerShip.InWarp && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName == "skipwarp";
						if (flag35)
						{
							PhotonNetwork.player.NickName = "null";
						}
						foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
						{
							bool flag36 = plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.PlayerLifeTime > 5f && plplayer.GetPhotonPlayer().GetScore() != plplayer.StartingShip.ShipID && plplayer.GetPlayerName(false).Contains("•") && PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo != null && Time.time - (PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo).LastAIAutoYellowAlertSetupTime > 2f && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot;
							if (flag36)
							{
								plplayer.StartingShip = (PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo);
								int startIndex2 = plplayer.GetPlayerName(false).LastIndexOf("•");
								plplayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
								{
									plplayer.StartingShip.ShipNameValue + " " + plplayer.GetPlayerName(false).Substring(startIndex2)
								});
								plplayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
								{
									(PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo).MyTLI.SubHubID,
									0
								});
							}
						}
						foreach (PLPlayer plplayer2 in PLServer.Instance.AllPlayers)
						{
							bool flag37 = plplayer2 != null && plplayer2.GetPhotonPlayer() != null && plplayer2.PlayerLifeTime > 10f && plplayer2.GetPlayerName(false).Contains("FREE") && plplayer2.GetPhotonPlayer().NickName != "locked";
							if (flag37)
							{
								int startIndex3 = plplayer2.GetPlayerName(false).LastIndexOf("•");
								plplayer2.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
								{
									plplayer2.StartingShip.ShipNameValue + " " + plplayer2.GetPlayerName(false).Substring(startIndex3)
								});
							}
						}
						bool flag38 = !PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.KeypadMinus);
						if (flag38)
						{
							PLServer.Instance.photonView.RPC("NetworkBeginWarp", PhotonTargets.All, new object[]
							{
								PLEncounterManager.Instance.PlayerShip.ShipID,
								PLServer.Instance.m_ShipCourseGoals[0],
								PLServer.Instance.GetEstimatedServerMs(),
								-1
							});
						}
						bool autoTarget2 = PLEncounterManager.Instance.PlayerShip.AutoTarget;
						if (autoTarget2)
						{
							PLEncounterManager.Instance.PlayerShip.AutoTarget = false;
						}
						foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
						{
							bool flag39 = plshipInfoBase != null && plshipInfoBase.IsAuxSystemActive(6) && plshipInfoBase.TagID == -23;
							if (flag39)
							{
								PLServer.Instance.photonView.RPC("SetAuxReactorConfigOff", PhotonTargets.All, new object[]
								{
									plshipInfoBase.ShipID,
									6
								});
							}
						}
					}
					bool flag40 = PhotonNetwork.isMasterClient && Time.time - __instance.LobbyPropertiesUpdateLastTime > 0.5f && PhotonNetwork.room != null;
					if (flag40)
					{
						__instance.LobbyPropertiesUpdateLastTime = Time.time;
						int num = 0;
						for (int i = 0; i < __instance.AllPlayers.Count; i++)
						{
							PLPlayer plplayer3 = __instance.AllPlayers[i];
							bool flag41 = plplayer3 != null && plplayer3.IsBot && plplayer3.TeamID == 0;
							if (flag41)
							{
								num++;
							}
						}
						Hashtable hashtable = new Hashtable();
						hashtable.Add("CurrentPlayersPlusBots", PhotonNetwork.room.PlayerCount + num);
						hashtable.Add("Private", PLNetworkManager.Instance.IsPrivateGame);
						bool flag42 = PLGlobal.Instance.Galaxy != null && PLGlobal.Instance.Galaxy.GenerationSettings != null;
						if (flag42)
						{
							hashtable.Add("GenSettings", PLGlobal.Instance.Galaxy.GenerationSettings.CreateDataString());
						}
						bool flag43 = PLEncounterManager.Instance.PlayerShip != null;
						if (flag43)
						{
							hashtable.Add("Ship_Name", PLEncounterManager.Instance.PlayerShip.ShipNameValue);
							hashtable.Add("Ship_Type", Plugin.myversion);
						}
						else
						{
							hashtable.Add("Ship_Name", PhotonNetwork.room.CustomProperties["Ship_Name"]);
							hashtable.Add("Ship_Type", PhotonNetwork.room.CustomProperties["Ship_Type"]);
						}
						bool flag44 = PhotonNetwork.room.CustomProperties.ContainsKey("SteamServerID");
						if (flag44)
						{
							hashtable.Add("SteamServerID", PhotonNetwork.room.CustomProperties["SteamServerID"]);
							CSteamID steamID = SteamUser.GetSteamID();
							string text = steamID.m_SteamID.ToString() + ",";
							foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
							{
								bool flag45 = photonPlayer != null && photonPlayer.SteamID != CSteamID.Nil && photonPlayer.SteamID != steamID;
								if (flag45)
								{
									text = text + photonPlayer.SteamID.m_SteamID.ToString() + ",";
								}
							}
							hashtable.Add("PlayerSteamIDs", text);
						}
						bool flag46 = hashtable.Count != PhotonNetwork.room.CustomProperties.Count;
						bool flag47 = !flag46;
						if (flag47)
						{
							foreach (object obj in hashtable.Keys)
							{
								string text2 = (string)obj;
								bool flag48 = PhotonNetwork.room.CustomProperties[text2].ToString() != hashtable[text2].ToString();
								if (flag48)
								{
									flag46 = true;
									break;
								}
							}
						}
						bool flag49 = flag46;
						if (flag49)
						{
							PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
						}
					}
					bool flag50 = Time.time - PLServer.Instance.LobbyPropertiesUpdateLastTime > 0.2f && PhotonNetwork.room != null;
					if (flag50)
					{
						int num2 = 0;
						foreach (PLShipInfoBase plshipInfoBase2 in PLEncounterManager.Instance.AllShips.Values)
						{
							bool flag51 = plshipInfoBase2 != null && plshipInfoBase2.GetIsPlayerShip();
							if (flag51)
							{
								num2++;
							}
						}
						PhotonNetwork.room.MaxPlayers = num2 * 5;
					}
				}
			}
		}
	}
}
