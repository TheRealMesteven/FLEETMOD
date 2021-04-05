using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLServer), "Update")]
    internal class UpdatePatch
    {
        static Dictionary<int, int> CachedFleet = Global.Fleet;
        public static void Postfix(PLServer __instance)
        {
            if (__instance != null && __instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLEncounterManager.Instance.PlayerShip != null)
            { // If In-game
                PLInGameUI.Instance.CurrentVersionLabel.text = "<color=ffff00>Fleetmod V2.0</color>" + PLNetworkManager.Instance.VersionString;
                if (!PhotonNetwork.isMasterClient || Global.devmode)
                { // If Crew
                    if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F1) && (PLNetworkManager.Instance.LocalPlayer.GetClassID() != 0 || Global.devmode))
                    { // Spawn Ship Keybind
                        PLMusic.PostEvent("play_sx_playermenu_click_major", PLServer.Instance.gameObject);
                        PLNetworkManager.Instance.MainMenu.CloseActiveMenu();
                        PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLCreateGameMenu(true));
                        PLTabMenu.Instance.TabMenuActive = false;
                    }
                }
            }
            if(PhotonNetwork.isMasterClient) //MasterClient Check for all masterclient code
            {
                if (CachedFleet != Global.Fleet)
                {
                    CachedFleet = Global.Fleet;
                    foreach (PLPlayer player in PLServer.Instance.AllPlayers)
                    { //for every player in PLServer.AllPlayers, check they are real and not the local player before sending ModMessage containing current Fleet.
                        PhotonPlayer photonplayer = player.GetPhotonPlayer();
                        if (!player.IsBot && photonplayer != PhotonNetwork.player)
                        {
                            PulsarPluginLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.SyncCrewIDs", photonplayer, new object[] { Global.Fleet });
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLServer), "LoginMessage")] //Initial Sync. If multiple messages exist consider making new Modmessage for initial sync
    class LoginMessagePatch
    {
        static void Postfix(PhotonPlayer newPhotonPlayer)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PulsarPluginLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.SyncCrewIDs", newPhotonPlayer, new object[] {
                    Global.Fleet
                });
            }
        }
    }
}