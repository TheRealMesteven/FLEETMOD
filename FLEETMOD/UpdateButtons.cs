using System;
using System.Collections.Generic;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x0200000D RID: 13
	[HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "UpdateButtons")]
	internal class UpdateButtons
	{
		// Token: 0x06000015 RID: 21 RVA: 0x00002C78 File Offset: 0x00000E78
		public static bool Prefix(PLOverviewPlayerInfoDisplay __instance, ref List<EPlayerButtonType2> ___ButtonsActiveTypes)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				___ButtonsActiveTypes.Clear();
				bool flag2 = __instance.MyPlayer == null;
				if (flag2)
				{
					bool flag3 = PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetClassID() != 0;
					if (flag3)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_PILOT);
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_SCI);
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_WEAP);
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_ENG);
					}
				}
				else
				{
					bool flag4 = PhotonNetwork.isMasterClient && __instance.MyPlayer != PLNetworkManager.Instance.LocalPlayer;
					if (flag4)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_REMOVE_BOT);
					}
					bool flag5 = SteamManager.Initialized && __instance.MyPlayer.SteamIDIsVisible && __instance.MyPlayer.GetPhotonPlayer() != null && __instance.MyPlayer.GetPhotonPlayer().SteamID != CSteamID.Nil;
					if (flag5)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_FRIEND);
					}
					bool flag6 = PLNetworkManager.Instance.LocalPlayer != __instance.MyPlayer && __instance.MyPlayer.TS_ValidClientID && PLVoiceChatManager.Instance.GetIsFullyStarted();
					if (flag6)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_MUTE);
					}
					bool isDebugBuild = Debug.isDebugBuild;
					if (isDebugBuild)
					{
						Debug.Log("QDB: --------------------------------------- CHANGED KICK ROUTINE ------------------------------");
					}
					bool flag7 = !__instance.MyPlayer.IsBot && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().IsMasterClient && __instance.MyPlayer.GetPhotonPlayer() != null && !__instance.MyPlayer.GetPhotonPlayer().IsMasterClient;
					if (flag7)
					{
						___ButtonsActiveTypes.Add(EPlayerButtonType2.E_KICK);
					}
					bool isDebugBuild2 = Debug.isDebugBuild;
					if (isDebugBuild2)
					{
						Debug.Log("QDB: --------------------------------------- CHANGED KICK ROUTINE ------------------------------");
					}
				}
				for (int i = 0; i < 4; i++)
				{
					bool flag8 = i < __instance.Buttons.Length;
					if (flag8)
					{
						__instance.Buttons[i].MyPID = __instance;
						bool flag9 = i < ___ButtonsActiveTypes.Count;
						if (flag9)
						{
							bool flag10 = __instance.Buttons[i].m_Label != null && !__instance.Buttons[i].m_Label.gameObject.activeSelf;
							if (flag10)
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
								bool flag11 = !SteamManager.Initialized || __instance.MyPlayer.GetPhotonPlayer() == null || !(__instance.MyPlayer.GetPhotonPlayer().SteamID != CSteamID.Nil) || !(__instance.MyPlayer != PLNetworkManager.Instance.LocalPlayer);
								if (flag11)
								{
									text = "-";
								}
								else
								{
									EFriendRelationship friendRelationship = SteamFriends.GetFriendRelationship(__instance.MyPlayer.GetPhotonPlayer().SteamID);
									bool flag12 = friendRelationship == EFriendRelationship.k_EFriendRelationshipFriend || friendRelationship == EFriendRelationship.k_EFriendRelationshipIgnoredFriend;
									if (flag12)
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
								bool flag13 = __instance.MyPlayer != null && __instance.MyPlayer.GetPhotonPlayer().NickName == "locked" && PhotonNetwork.isMasterClient && __instance.MyPlayer.GetClassID() != 0 && __instance.MyPlayer.GetPhotonPlayer().GetScore() > 0;
								if (flag13)
								{
									text = "Release";
								}
								else
								{
									bool flag14 = __instance.MyPlayer != null && __instance.MyPlayer.GetPhotonPlayer().NickName != "locked" && PhotonNetwork.isMasterClient && __instance.MyPlayer.GetClassID() != 0 && __instance.MyPlayer.GetPhotonPlayer().GetScore() > 0;
									if (flag14)
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
								bool flag15 = __instance.MyPlayer != null && __instance.MyPlayer.TS_IsMuted;
								if (flag15)
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
							bool flag16 = __instance.Buttons[i].m_Label != null && __instance.Buttons[i].m_Label.gameObject.activeSelf;
							if (flag16)
							{
								__instance.Buttons[i].m_Label.gameObject.SetActive(false);
							}
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
