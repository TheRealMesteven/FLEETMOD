using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.UI;
using UnityEngine;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLInGameUI), "WarpSkipButtonClicked")]
    internal class WarpSkipButton
    {
        // Skips Warp when F8 Pressed
        public static bool Prefix()
        {
            if (!MyVariables.isrunningmod) return true;
            if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
            {
                PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.photonView.RPC("SkipWarp", PhotonTargets.All, Array.Empty<object>());
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PLServer), "Update")]
    internal class WarpSkip
    {
        // Sets Admirals Nickname (Synced with all Clients) to skipwarp to allow Non-Admirals to know when they can skip warp
        public static void Postfix(PLServer __instance)
        {
            if (!MyVariables.isrunningmod || __instance == null || PLNetworkManager.Instance == null || PLEncounterManager.Instance == null) return;
            if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLServer.Instance.GameHasStarted)
            {
                if (PLServer.Instance.AllPlayersLoaded() && Mathf.Abs((float)((long)PLServer.Instance.GetEstimatedServerMs() - (long)PLEncounterManager.Instance.PlayerShip.LastBeginWarpServerTime)) > 16000f && PhotonNetwork.player.NickName != "skipwarp")
                {
                    PhotonNetwork.player.NickName = "skipwarp";
                }
                if (!PLEncounterManager.Instance.PlayerShip.InWarp && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName == "skipwarp")
                {
                    PhotonNetwork.player.NickName = "null";
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLInGameUI), "Update")]
    internal class WarpSkipInGameUI
    {
        // Display Skip Warp Label to all Captains
        public static void Postfix(PLInGameUI __instance, ref List<PLPlayer> ___relevantPlayersForCrewStatus, ref PLCachedFormatString<string> ___cSkipWarpLabel, ref Text[] ___CrewStatusSlots_HPs, ref Text[] ___CrewStatusSlots_Names, ref Image[] ___CrewStatusSlots_BGs, ref Image[] ___CrewStatusSlots_Fills, ref Image[] ___CrewStatusSlots_TalkingImages, ref Image[] ___CrewStatusSlots_SlowFills)
        {
            if (!MyVariables.isrunningmod || PLServer.Instance == null || PLNetworkManager.Instance == null || PLNetworkManager.Instance.LocalPlayer == null) return;
            if (PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLServer.Instance.GetPlayerFromPlayerID(0) != null)
            {
                // If the skipwarp label should appear (Host nickname as skipwarp indicates we're in warp and can skip)
                if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
                {
                    PLGlobal.SafeLabelSetText(__instance.SkipWarpLabel, ___cSkipWarpLabel.ToString(PLInput.Instance.GetPrimaryKeyStringForAction(PLInputBase.EInputActionName.skip_warp, true)));
                    __instance.SkipWarpLabel.enabled = true;
                }
                else
                {
                    __instance.SkipWarpLabel.enabled = false;
                }

                // Code to ensure Skipwarp Label only appears as "skipwarp" if in-game as a pawn (Not on main menu etc)
                string a2 = "";
                if (PLCameraSystem.Instance != null)
                {
                    a2 = PLCameraSystem.Instance.GetModeString();
                }
                if (UnityEngine.Random.Range(0, 20) == 0 || PLServer.Instance == null)
                {
                    string text = "";
                    if (__instance.ControlsText.enabled)
                    {
                        if (a2 == "LocalPawn")
                        {
                            if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp")
                            {
                                text = text + "<color=#AAAAAA><color=#ffff00>" + PLInput.Instance.GetPrimaryKeyStringForAction(PLInputBase.EInputActionName.skip_warp, true) + "</color> Skip Warp</color>\n";
                            }
                        }
                        else
                        {
                            text = "";
                        }
                    }
                    PLGlobal.SafeLabelSetText(__instance.ControlsText, text);
                }
            }
        }
    }
}
