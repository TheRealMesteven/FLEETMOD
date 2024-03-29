﻿using System;
using ExitGames.Client.Photon;
using HarmonyLib;
using PulsarModLoader.Utilities;
using Steamworks;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "Update")]
	internal class Update
	{
		public static void Postfix(PLServer __instance)
		{
            if (Variables.isrunningmod)
			{
				if (__instance != null && __instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLEncounterManager.Instance.PlayerShip != null)
				{
                    PLEncounterManager.Instance.PlayerShip.TagID = -23;
					PLInGameUI.Instance.CurrentOrdersLabel.enabled = true;
					PLInGameUI.Instance.CurrentOrdersLabel.resizeTextForBestFit = true;
					PLInGameUI.Instance.CurrentOrdersLabel.supportRichText = true;
					PLInGameUI.Instance.CurrentOrdersLabel.text = "<color=#ffffff>Your Admiral </color><color=#ffff00>\n" + PLServer.Instance.GetPlayerFromPlayerID(0).GetPlayerName(true) + "</color>";

					///<summary>
					/// Below code creates Outpost dialog for assigning captains to unmanned ships.
					///</summary>
					if (PLServer.GetCurrentSector().Name.Contains("W.D. HUB") || PLServer.GetCurrentSector().Name.Contains("Outpost 448") || PLServer.GetCurrentSector().Name.Contains("The Estate") || PLServer.GetCurrentSector().Name.Contains("Cornelia Station") || PLServer.GetCurrentSector().Name.Contains("The Burrow") || PLServer.GetCurrentSector().Name.Contains("The Harbor") || PLServer.GetCurrentSector().Name.Contains("Fluffy"))
					{
						if (Variables.DialogGenerated != true && PhotonNetwork.isMasterClient)
                        {
                            Variables.DialogGenerated = true;
                            var go = new UnityEngine.GameObject("FleetManager_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                            go.AddComponent<Interface.Dialogs.FleetManager>(); // Also TODO: Rename local vars...
                        }
					}
					///
					if (PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && !PLTabMenu.Instance.TabMenuActive && (Interface.Tab.UpdatePLTabMenu.CrewPage != 0 || Interface.Tab.UpdatePLTabMenu.ChangeClassPage))
					{
                        Interface.Tab.UpdatePLTabMenu.CrewPage = 0;
                        Interface.Tab.UpdatePLTabMenu.ChangeClassPage = false;
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
						/*if (MyVariables.BriggedCrew.Contains(PLNetworkManager.Instance.LocalPlayer.GetPlayerID()) && !MyVariables.TeleportedBrig 
							&& PLNetworkManager.Instance.LocalPlayer.GetPawn().Lifetime > 1f 
							&& PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip)
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

							MyVariables.TeleportedBrig = true;
							foreach (PLDoor pldoor in PLDoor.AllDoors)
							{
								if (pldoor != null)
								{
									pldoor.OpenRange = 0f;
								}
							}
						}
						
						if (!MyVariables.BriggedCrew.Contains(PLNetworkManager.Instance.LocalPlayer.GetPlayerID()) && MyVariables.TeleportedBrig)
						{
							MyVariables.TeleportedBrig = false;
							PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
							{
								"<color=#c0c0c0><size=15>You Have Been Released From The Brig.</size></color>"
							})));
							foreach (PLDoor pldoor2 in PLDoor.AllDoors)
							{
								if (pldoor2 != null)
								{
									pldoor2.OpenRange = 2.5f;
								}
							}
						}
						*/
					}
					if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLServer.Instance.GameHasStarted)
					{
						bool SurvivalBonusUpdate = false;
						foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
						{
							if (!Variables.survivalBonusDict.ContainsKey(plplayer.GetPlayerID()))
							// if playerid doesn't exist in dictionary add playerid to dict and set hp bonus to 0
							{
								Variables.survivalBonusDict.Add(plplayer.GetPlayerID(), 0);
								SurvivalBonusUpdate = true;
							}

							if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.PlayerLifeTime > 5f && plplayer.GetPhotonPlayer().GetScore() != plplayer.StartingShip.ShipID && PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo != null && Time.time - (PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo).LastAIAutoYellowAlertSetupTime > 2f && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot)
							{
								plplayer.StartingShip = (PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo);
								plplayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
								{
									(PLEncounterManager.Instance.GetShipFromID(plplayer.GetPhotonPlayer().GetScore()) as PLShipInfo).MyTLI.SubHubID,
									0
								});
							}
                            /*if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.PlayerLifeTime > 10f && plplayer.GetPlayerName(false).Contains("FREE") && plplayer.GetPhotonPlayer().NickName != "locked")
                            {
                                plplayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
                                {
                                    plplayer.StartingShip.ShipNameValue + " " + plplayer.GetPlayerName(false).Substring(plplayer.GetPlayerName(false).LastIndexOf("•"))
                                });
                            }*/
                        }
						if (SurvivalBonusUpdate)
						{
                            //Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, "[SURVIVAL BONUS UPDATE] - Update Mod Message");
                            ModMessages.ServerUpdateVariables.UpdateClients();
						}
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
					}
				}
			}
		}
	}
}
