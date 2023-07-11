using System;
using System.Collections.Generic;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD.Interface.Tab
{
    [HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "UpdateButtons")]
    internal class UpdateButtons
    {
        private static string GetStringFromButtonType(PLOverviewPlayerInfoDisplay __instance, EPlayerButtonType2 inType)
        {
            switch (inType)
            {
                case EPlayerButtonType2.E_VIEW_TALENTS:
                    return "Talents";
                case EPlayerButtonType2.E_VIEW_ITEMS:
                    return "Items";
                case EPlayerButtonType2.E_ADD_BOT_PILOT:
                    return "Add PiBot";
                case EPlayerButtonType2.E_ADD_BOT_SCI:
                    return "Add SciBot";
                case EPlayerButtonType2.E_ADD_BOT_WEAP:
                    return "Add WeapBot";
                case EPlayerButtonType2.E_ADD_BOT_ENG:
                    return "Add EngBot";
                case EPlayerButtonType2.E_CHANGE_TO_PILOT:
                    return "Pilot";
                case EPlayerButtonType2.E_CHANGE_TO_SCI:
                    return "Science";
                case EPlayerButtonType2.E_CHANGE_TO_WEAP:
                    return "Weapons";
                case EPlayerButtonType2.E_CHANGE_TO_ENG:
                    return "Engineer";
                case EPlayerButtonType2.E_ADD_FRIEND:
                    if (!SteamManager.Initialized || __instance.MyPlayer.GetPhotonPlayer() == null || !(__instance.MyPlayer.GetPhotonPlayer().SteamID != CSteamID.Nil) || !(__instance.MyPlayer != PLNetworkManager.Instance.LocalPlayer))
                    {
                        return "-";
                    }
                    EFriendRelationship friendRelationship = SteamFriends.GetFriendRelationship(__instance.MyPlayer.GetPhotonPlayer().SteamID);
                    if (friendRelationship == EFriendRelationship.k_EFriendRelationshipFriend || friendRelationship == EFriendRelationship.k_EFriendRelationshipIgnoredFriend)
                    {
                        return "View Profile";
                    }
                    return "Add Friend";
                case EPlayerButtonType2.E_REMOVE_BOT:
                    /*
                    if (__instance.MyPlayer != null && MyVariables.BriggedCrew.Contains(__instance.MyPlayer.GetPlayerID()) && PhotonNetwork.isMasterClient && __instance.MyPlayer.GetPhotonPlayer().GetScore() > 0)
                    {
                        return "Release";
                    }
                    if (__instance.MyPlayer != null && !MyVariables.BriggedCrew.Contains(__instance.MyPlayer.GetPlayerID()) && PhotonNetwork.isMasterClient && __instance.MyPlayer.GetClassID() != 0 && __instance.MyPlayer.GetPlayerName(false).Contains("•") && __instance.MyPlayer.GetPhotonPlayer().GetScore() > 0)
                    {
                        return "To Brig";
                    }
                    */
                    return "Remove Bot";
                case EPlayerButtonType2.E_KICK:
                    return "Kick";
                case EPlayerButtonType2.E_MUTE:
                    if (__instance.MyPlayer != null && __instance.MyPlayer.TS_IsMuted)
                    {
                        return "Unmute";
                    }
                    return "Mute";
                default:
                    return "";
            }
        }

        public static bool Prefix(PLOverviewPlayerInfoDisplay __instance, ref List<EPlayerButtonType2> ___ButtonsActiveTypes)
        {
            if (!Variables.isrunningmod || PLNetworkManager.Instance.LocalPlayer == null)
            {
                return true;
            }
            else
            {
                ___ButtonsActiveTypes.Clear();
                if (__instance.MyPlayer != null)
                {
                    if (Interface.Tab.UpdatePLTabMenu.ChangeClassPage)
                    {
                        ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_CHANGE_TO_PILOT);
                        ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_CHANGE_TO_SCI);
                        ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_CHANGE_TO_WEAP);
                        ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_CHANGE_TO_ENG);
                    }
                    else if (__instance.MyPlayer == PLNetworkManager.Instance.LocalPlayer && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
                    {
                        ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_PILOT);
                        ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_SCI);
                        ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_WEAP);
                        ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_ADD_BOT_ENG);
                    }
                    else
                    {
                        if (__instance.MyPlayer.IsBot && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0 && (PhotonNetwork.isMasterClient || PLNetworkManager.Instance.LocalPlayer.StartingShip == __instance.MyPlayer.StartingShip))
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
                        if (!__instance.MyPlayer.IsBot && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().IsMasterClient && __instance.MyPlayer.GetPhotonPlayer() != null && !__instance.MyPlayer.GetPhotonPlayer().IsMasterClient)
                        {
                            ___ButtonsActiveTypes.Add(EPlayerButtonType2.E_KICK);
                        }
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

                            __instance.Buttons[i].m_Label.text = GetStringFromButtonType(__instance, ___ButtonsActiveTypes[i]);
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
