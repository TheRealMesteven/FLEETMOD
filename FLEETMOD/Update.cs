using System;
using ExitGames.Client.Photon;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "Update")]
	internal class Update
	{
		public static void Postfix(PLServer __instance)
		{
            if (MyVariables.isrunningmod)
			{
				if (__instance != null && __instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLEncounterManager.Instance.PlayerShip != null)
				{
                    PulsarModLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.HostUpdateVariables", PhotonTargets.MasterClient, new object[] { });
					PLEncounterManager.Instance.PlayerShip.TagID = -23;
					PLInGameUI.Instance.CurrentOrdersLabel.enabled = true;
					PLInGameUI.Instance.CurrentOrdersLabel.resizeTextForBestFit = true;
					PLInGameUI.Instance.CurrentOrdersLabel.supportRichText = true;
					PLInGameUI.Instance.CurrentOrdersLabel.text = "<color=#ffffff>Your Admiral </color><color=#ffff00>\n" + PLServer.Instance.GetPlayerFromPlayerID(0).GetPlayerName(true) + "</color>";
					PLInGameUI.Instance.CurrentVersionLabel.text = Plugin.myversion;
					PLInGameUI.Instance.ControlsText.enabled = true;
					string str = "<color=#FFFFFF>";

					// Switched ifs to fancy switch statement
					if (!PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains(" • "))
                    {
						PLNetworkManager.Instance.LocalPlayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
						{
							PLEncounterManager.Instance.PlayerShip.ShipNameValue + " • " + PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false)
						});
					}
					if (!PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains(PLEncounterManager.Instance.PlayerShip.ShipNameValue + " • "))
                    {
						PLNetworkManager.Instance.LocalPlayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
						{
							PLEncounterManager.Instance.PlayerShip.ShipNameValue + " • " + PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Substring(PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).LastIndexOf("•" + 2))
						});
					}
					switch (PLNetworkManager.Instance.LocalPlayer.GetClassID())
					{
						case 0:
							if (PhotonNetwork.isMasterClient) str = "<color=#0066FF>";
							else str = "<color=#ffff00>";
							break;
						case 1:
							str = "<color=#FFFFFF>";
							break;
						case 2:
							str = "<color=#00FF00>";
							break;
						case 3:
							str = "<color=#FF0000>";
							break;
						case 4:
							str = "<color=#FF6600>";
							break;

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
					///<summary>
					/// Below code creates Outpost dialog for assigning captains to unmanned ships.
					///</summary>
					if (PLServer.GetCurrentSector().Name.Contains("W.D. HUB") || PLServer.GetCurrentSector().Name.Contains("Outpost 448") || PLServer.GetCurrentSector().Name.Contains("The Estate") || PLServer.GetCurrentSector().Name.Contains("Cornelia Station") || PLServer.GetCurrentSector().Name.Contains("The Burrow") || PLServer.GetCurrentSector().Name.Contains("The Harbor"))
					{
                        if (MyVariables.DialogGenerated != true && PhotonNetwork.isMasterClient)
                        {
                            MyVariables.DialogGenerated = true;
                            var go = new UnityEngine.GameObject("FleetManager_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                            go.AddComponent<Interface.Dialogs.FleetManager>(); // Also TODO: Rename local vars...
                        }
                    }
					if (!MyVariables.FuelDialog && PhotonNetwork.isMasterClient)
                    {
						MyVariables.FuelDialog = true;
						var go = new UnityEngine.GameObject("FuelTransfer_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
						go.AddComponent<Interface.Dialogs.FuelTransfer>(); // Also TODO: Rename local vars...
					}
                    ///
					if (PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && !PLTabMenu.Instance.TabMenuActive && PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp != 0)
					{
						PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp = 0;
					}
					// This is where warp range bindings occur
					if (__instance != null && !PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip != null && PLEncounterManager.Instance.PlayerShip != null)
					{
						if (PLNetworkManager.Instance.LocalPlayer.StartingShip != null && PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip != null && PLNetworkManager.Instance.LocalPlayer.StartingShip.WarpTravelDist != PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.WarpTravelDist)
						{
							PLNetworkManager.Instance.LocalPlayer.StartingShip.WarpTravelDist = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.WarpTravelDist;
						} // This is end of warp range bindings
						PLNetworkManager.Instance.LocalPlayer.StartingShip.LastBeginWarpServerTime = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.LastBeginWarpServerTime;
                        /*
                        bool flag12 = !PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F1) && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked" && PLNetworkManager.Instance.LocalPlayer.GetClassID() != 0;
                        if (flag12)
						{
							PLMusic.PostEvent("play_sx_playermenu_click_major", PLServer.Instance.gameObject);
							PLNetworkManager.Instance.MainMenu.CloseActiveMenu();
							PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLCreateGameMenu(true));
							PLTabMenu.Instance.TabMenuActive = false;
						}
                        */
						if (PLNetworkManager.Instance.LocalPlayer.StartingShip != null && PLNetworkManager.Instance.LocalPlayer.StartingShip.TeamID != 0)
						{
							PLNetworkManager.Instance.LocalPlayer.StartingShip.TeamID = 0;
						}
						if (PLNetworkManager.Instance.LocalPlayer.GetPawn() != null && PLNetworkManager.Instance.LocalPlayer.GetPawn().Lifetime > 2f && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip != null)
						{
							if (PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.GetIsPlayerShip() && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.TeamID != -1 && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip != PLNetworkManager.Instance.LocalPlayer.StartingShip)
							{
								PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.TeamID = -1;
							}
						}
						if (PLEncounterManager.Instance.PlayerShip.AutoTarget)
						{
							PLEncounterManager.Instance.PlayerShip.AutoTarget = false;
						}
						if ((PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("BRIG") && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked" && PLNetworkManager.Instance.LocalPlayer.GetPawn().Lifetime > 1f && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip) || (!PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("BRIG") && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName == "locked" && !PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("FREE") && PLNetworkManager.Instance.LocalPlayer.GetPawn().Lifetime > 5f && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip))
						{
							if (!PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("BRIG"))
							{
								PLNetworkManager.Instance.LocalPlayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
								{
									"BRIG " + PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Substring(PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).LastIndexOf("•"))
								});
							}
							PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
							{
								"<color=#c0c0c0><size=15>You Have Been Confined To The Brig By The Admiral.\n\n Please Wait Here Until The Admiral Releases You.</size></color>"
							})));

							var playerPawn = PLNetworkManager.Instance.LocalPlayer.GetPawn(); // Getting LocalPlayerPawn Instance
							// Switched ifs to fancy switch statement
							switch (PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.ShipTypeID)
                            {
								case EShipType.E_OUTRIDER:
									playerPawn.transform.position = new Vector3(12.5f, -406f, -2f);
									break;

								case EShipType.E_INTREPID:
									playerPawn.transform.position = new Vector3(9.4f, -400.9f, -13.8f);
									break;

								case EShipType.E_ROLAND:
									playerPawn.transform.position = new Vector3(8.8f, -391f, -30.8f);
									break;

								case EShipType.OLDWARS_HUMAN:
									playerPawn.transform.position = new Vector3(-14.8f, -365.8f, 38.1f);
									break;

								case EShipType.E_WDCRUISER:
									playerPawn.transform.position = new Vector3(-9.4f, -397f, -5f);
									break;

								case EShipType.E_DESTROYER:
									playerPawn.transform.position = new Vector3(0.2f, 405f, 14.1f);
									break;

								case EShipType.E_ANNIHILATOR:
									playerPawn.transform.position = new Vector3(-5.6f, -400f, -1.9f);
									break;

								case EShipType.E_CIVILIAN_STARTING_SHIP:
									playerPawn.transform.position = new Vector3(-5.6f, -384.5f, 24f);
									break;

								case EShipType.E_STARGAZER:
									playerPawn.transform.position = new Vector3(-7.3f, -394.5f, 13.6f);
									break;

								case EShipType.E_CARRIER:
									playerPawn.transform.position = new Vector3(1.2f, -386f, 18.4f);
									break;

								case EShipType.OLDWARS_SYLVASSI:
									playerPawn.transform.position = new Vector3(-16.1f, -397.5f, -15.5f);
									break;

								case EShipType.E_INTREPID_SC:
									playerPawn.transform.position = new Vector3(9.4f, -400.9f, -13.8f);
									break;
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
						if (PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Contains("FREE") && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "open")
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
					if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLServer.Instance.GameHasStarted)
					{
						if (PLNetworkManager.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PhotonNetwork.isMasterClient && PLServer.Instance != null && PLServer.Instance.AllPlayersLoaded() && PLEncounterManager.Instance.PlayerShip != null && Mathf.Abs((float)((long)PLServer.Instance.GetEstimatedServerMs() - (long)PLEncounterManager.Instance.PlayerShip.LastBeginWarpServerTime)) > 16000f && PhotonNetwork.player.NickName != "skipwarp")
						{
							PhotonNetwork.player.NickName = "skipwarp";
						}
						if (!PLEncounterManager.Instance.PlayerShip.InWarp && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName == "skipwarp")
						{
							PhotonNetwork.player.NickName = "null";
						}
						foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
						{
							if (!MyVariables.survivalBonusDict.ContainsKey(plplayer.GetPlayerID()))
							// if playerid doesn't exist in dictionary add playerid to dict and set hp bonus to 0
							{
								MyVariables.survivalBonusDict.Add(plplayer.GetPlayerID(), 0);
							}

							if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.PlayerLifeTime > 5f && plplayer.GetPhotonPlayer().GetScore() != plplayer.StartingShip.ShipID && plplayer.GetPlayerName(false).Contains("•") && PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo != null && Time.time - (PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo).LastAIAutoYellowAlertSetupTime > 2f && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot)
							{
								plplayer.StartingShip = (PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo);
								plplayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
								{
									plplayer.StartingShip.ShipNameValue + " " + plplayer.GetPlayerName(false).Substring(plplayer.GetPlayerName(false).LastIndexOf("•"))
								});
								plplayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
								{
									(PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo).MyTLI.SubHubID,
									0
								});
							}
                            if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.PlayerLifeTime > 10f && plplayer.GetPlayerName(false).Contains("FREE") && plplayer.GetPhotonPlayer().NickName != "locked")
                            {
                                plplayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
                                {
                                    plplayer.StartingShip.ShipNameValue + " " + plplayer.GetPlayerName(false).Substring(plplayer.GetPlayerName(false).LastIndexOf("•"))
                                });
                            }
                        }
                        /*foreach (PhotonPlayer Photon in MyVariables.FleetmodPhoton)
                        {
                            try
                            {
                                MyVariables.FleetmodPlayer.Add(PLServer.GetPlayerForPhotonPlayer(Photon));
                                MyVariables.FleetmodPhoton.Remove(Photon);
                            }
                            catch { }
                        }*/
						if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.KeypadMinus) && PLServer.Instance.ClientHasFullStarmap)
						{
							PLServer.Instance.photonView.RPC("NetworkBeginWarp", PhotonTargets.All, new object[]
							{
								PLEncounterManager.Instance.PlayerShip.ShipID,
                                PLStarmap.Instance.CurrentShipPath[1],
								PLServer.Instance.GetEstimatedServerMs(),
								-1
							});
						}
						if (PLEncounterManager.Instance.PlayerShip.AutoTarget)
						{
							PLEncounterManager.Instance.PlayerShip.AutoTarget = false;
						}
						foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
						{
							if (plshipInfoBase != null && plshipInfoBase.IsAuxSystemActive(6) && plshipInfoBase.TagID == -23)
							{
								PLServer.Instance.photonView.RPC("SetAuxReactorConfigOff", PhotonTargets.All, new object[]
								{
									plshipInfoBase.ShipID,
									6
								});
							}
						}
					}
					if (PhotonNetwork.isMasterClient && Time.time - __instance.LobbyPropertiesUpdateLastTime > 0.5f && PhotonNetwork.room != null)
					{
						__instance.LobbyPropertiesUpdateLastTime = Time.time;
						int num = 0;
						for (int i = 0; i < __instance.AllPlayers.Count; i++)
						{
							PLPlayer plplayer3 = __instance.AllPlayers[i];
							if (plplayer3 != null && plplayer3.IsBot && plplayer3.TeamID == 0)
							{
								num++;
							}
						}
						Hashtable hashtable = new Hashtable();
						hashtable.Add("CurrentPlayersPlusBots", PhotonNetwork.room.PlayerCount + num);
						hashtable.Add("Private", PLNetworkManager.Instance.IsPrivateGame);
						if (PLGlobal.Instance.Galaxy != null && PLGlobal.Instance.Galaxy.GenerationSettings != null)
						{
							hashtable.Add("GenSettings", PLGlobal.Instance.Galaxy.GenerationSettings.CreateDataString());
						}
						if (PLEncounterManager.Instance.PlayerShip != null)
						{
							hashtable.Add("Ship_Name", PLEncounterManager.Instance.PlayerShip.ShipNameValue);
							hashtable.Add("Ship_Type", Plugin.myversion);
						}
						else
						{
							hashtable.Add("Ship_Name", PhotonNetwork.room.CustomProperties["Ship_Name"]);
							hashtable.Add("Ship_Type", PhotonNetwork.room.CustomProperties["Ship_Type"]);
						}
						if (PhotonNetwork.room.CustomProperties.ContainsKey("SteamServerID"))
						{
							hashtable.Add("SteamServerID", PhotonNetwork.room.CustomProperties["SteamServerID"]);
							CSteamID steamID = SteamUser.GetSteamID();
							string text = steamID.m_SteamID.ToString() + ",";
							foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
							{
								if (photonPlayer != null && photonPlayer.SteamID != CSteamID.Nil && photonPlayer.SteamID != steamID)
								{
									text = text + photonPlayer.SteamID.m_SteamID.ToString() + ",";
								}
							}
							hashtable.Add("PlayerSteamIDs", text);
						}
						bool flag46 = hashtable.Count != PhotonNetwork.room.CustomProperties.Count;
						if (!flag46)
						{
							foreach (object obj in hashtable.Keys)
							{
								string text2 = (string)obj;
								if (PhotonNetwork.room.CustomProperties[text2].ToString() != hashtable[text2].ToString())
								{
									flag46 = true;
									break;
								}
							}
						}
						if (flag46)
						{
							PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
						}
					}
					if (Time.time - PLServer.Instance.LobbyPropertiesUpdateLastTime > 0.2f && PhotonNetwork.room != null)
					{
						int num2 = 0;
						foreach (PLShipInfoBase plshipInfoBase2 in PLEncounterManager.Instance.AllShips.Values)
						{
							if (plshipInfoBase2 != null && plshipInfoBase2.GetIsPlayerShip())
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
