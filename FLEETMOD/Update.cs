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
            if (__instance != null && __instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLEncounterManager.Instance.PlayerShip != null)
            { // If In-game
                if (!PhotonNetwork.isMasterClient)
                { // If Crew
                    if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F1) && PLNetworkManager.Instance.LocalPlayer.GetClassID() != 0)
                    { // Spawn Ship Keybind
                        PLMusic.PostEvent("play_sx_playermenu_click_major", PLServer.Instance.gameObject);
                        PLNetworkManager.Instance.MainMenu.CloseActiveMenu();
                        PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLCreateGameMenu(true));
                        PLTabMenu.Instance.TabMenuActive = false;
                    }
                }
            }
        }
    }
}