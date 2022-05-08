using System;
using System.Collections.Generic;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "UpdateButtons")]
	internal class UpdateButtons
	{
		public static bool Prefix(PLOverviewPlayerInfoDisplay __instance, ref List<EPlayerButtonType2> ___ButtonsActiveTypes)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (__instance.MyPlayer == null)
				{
					if (PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetClassID() != 0)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_PILOT);
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_SCI);
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_WEAP);
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_ENG);
					}
				}
				else
				{
					if (PhotonNetwork.isMasterClient && __instance.MyPlayer != PLNetworkManager.Instance.LocalPlayer)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_REMOVE_BOT);
					}
					if (SteamManager.Initialized && __instance.MyPlayer.SteamIDIsVisible && __instance.MyPlayer.GetPhotonPlayer() != null && __instance.MyPlayer.GetPhotonPlayer().SteamID != CSteamID.Nil)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_FRIEND);
					}
					if (PLNetworkManager.Instance.LocalPlayer != __instance.MyPlayer && __instance.MyPlayer.TS_ValidClientID && PLVoiceChatManager.Instance.GetIsFullyStarted())
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_MUTE);
					}
					if (Debug.isDebugBuild)
					{
						Debug.Log("QDB: --------------------------------------- CHANGED KICK ROUTINE ------------------------------");
					}
					if (!__instance.MyPlayer.IsBot && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().IsMasterClient && __instance.MyPlayer.GetPhotonPlayer() != null && !__instance.MyPlayer.GetPhotonPlayer().IsMasterClient)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_KICK);
					}
					if (Debug.isDebugBuild)
					{
						Debug.Log("QDB: --------------------------------------- CHANGED KICK ROUTINE ------------------------------");
					}
				}
				for (int i = 0; i < 4; i++)
				{
					if (i < __instance.Buttons.Length)
					{
						__instance.Buttons[i].MyPID = __instance;
						if (i < ___ButtonsActiveTypes.Count)
						{
							if (__instance.Buttons[i].m_Label != null && !__instance.Buttons[i].m_Label.gameObject.activeSelf)
							{
								__instance.Buttons[i].m_Label.gameObject.SetActive(true);
							}
							string text;
							switch (___ButtonsActiveTypes[i])
							{
							case EPlayerButtonType2.E_VIEW_TALENTS:
								text = "Talents";
								break;
							case EPlayerButtonType2.E_VIEW_ITEMS:
								text = "Items";
								break;
							case EPlayerButtonType2.E_ADD_BOT_PILOT:
								text = "Pilot";
								break;
							case EPlayerButtonType2.E_ADD_BOT_SCI:
								text = "Science";
								break;
							case EPlayerButtonType2.E_ADD_BOT_WEAP:
								text = "Weapons";
								break;
							case EPlayerButtonType2.E_ADD_BOT_ENG:
								text = "Engineer";
								break;
							case EPlayerButtonType2.E_ADD_FRIEND:
							{
								if (!SteamManager.Initialized || __instance.MyPlayer.GetPhotonPlayer() == null || !(__instance.MyPlayer.GetPhotonPlayer().SteamID != CSteamID.Nil) || !(__instance.MyPlayer != PLNetworkManager.Instance.LocalPlayer))
								{
									text = "-";
								}
								else
								{
									EFriendRelationship friendRelationship = SteamFriends.GetFriendRelationship(__instance.MyPlayer.GetPhotonPlayer().SteamID);
									if (friendRelationship == EFriendRelationship.k_EFriendRelationshipFriend || friendRelationship == EFriendRelationship.k_EFriendRelationshipIgnoredFriend)
									{
										text = "View Profile";
									}
									else
									{
										text = "Add Friend";
									}
								}
								break;
							}
							case EPlayerButtonType2.E_REMOVE_BOT:
							{
								if ( __instance.MyPlayer != null && __instance.MyPlayer.GetPhotonPlayer().NickName == "locked" && PhotonNetwork.isMasterClient && __instance.MyPlayer.GetClassID() != 0 && __instance.MyPlayer.GetPhotonPlayer().GetScore() > 0)
								{
									text = "Release";
								}
								else
								{
									if (__instance.MyPlayer != null && __instance.MyPlayer.GetPhotonPlayer().NickName != "locked" && PhotonNetwork.isMasterClient && __instance.MyPlayer.GetClassID() != 0 && __instance.MyPlayer.GetPlayerName(false).Contains("•") && __instance.MyPlayer.GetPhotonPlayer().GetScore() > 0)
									{
										text = "To Brig";
									}
									else
									{	
										text = "";
									}
								}
								break;
							}
							case EPlayerButtonType2.E_KICK:
								text = "Kick";
								break;
							case EPlayerButtonType2.E_MUTE:
							{
								if (__instance.MyPlayer != null && __instance.MyPlayer.TS_IsMuted)
								{
									text = "Unmute";
								}
								else
								{
									text = "Mute";
								}
								break;
							}
							default:
								text = "";
								break;
							}
							__instance.Buttons[i].m_Label.text = text;
						}
						else
						{
							if (__instance.Buttons[i].m_Label != null && __instance.Buttons[i].m_Label.gameObject.activeSelf)
							{
								__instance.Buttons[i].m_Label.gameObject.SetActive(false);
							}
						}
					}
				}
				return false;
			}
		}
	}
}
