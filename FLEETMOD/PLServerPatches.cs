using System;
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
                PLEncounterManager.Instance.PlayerShip.TagID = -23; // Want to remove but is required for Hostile Ship Patch
                if (PhotonNetwork.isMasterClient && !Global.ishostshipsetup)
                { // Adding host ship to the fleet dictionary.
                    int CrewID = Global.GetLowestUncrewedID();
                    Global.Fleet.Add(CrewID, PLEncounterManager.Instance.PlayerShip.ShipID);
                    Global.ishostshipsetup = true;
                }
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
            if (PhotonNetwork.isMasterClient) //MasterClient Check for all masterclient code
            {
                if (CachedFleet != Global.Fleet)
                {
                    CachedFleet = Global.Fleet;
                    foreach (PLPlayer player in PLServer.Instance.AllPlayers)
                    { //for every player in PLServer.AllPlayers, check they are real and not the local player before sending ModMessage containing current Fleet.
                        PhotonPlayer photonplayer = player.GetPhotonPlayer();
                        if (!player.IsBot && photonplayer != PhotonNetwork.player)
                        {
                            PulsarPluginLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.SyncCrewIDs", photonplayer, new object[] { Global.Fleet, Global.PlayerCrewList });
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
                    Global.Fleet,
                    Global.PlayerCrewList
                });
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "Start")]
    class StartPatch
    {
        static void Postfix()
        {
            Global.Fleet = new Dictionary<int, int>();
            Global.PlayerCrewList = new Dictionary<int, int>();
        }
    }
    [HarmonyPatch(typeof(PLServer), "ClaimShip")]
    class ClaimPatch
    {
        static void Prefix(PLServer __instance, ref int inShipID)
        {
            PLShipInfo plshipInfo = null;
            plshipInfo = (PLEncounterManager.Instance.GetShipFromID(inShipID) as PLShipInfo);
            if (plshipInfo != null && plshipInfo != PLEncounterManager.Instance.PlayerShip)
            {
                foreach (PLPlayer plplayer in __instance.AllPlayers)
                {//Enemy unclaims player ship
                    if (plplayer != null && plplayer.StartingShip == plshipInfo)
                    {
                        plplayer.StartingShip = null;
                    }
                }
                if (plshipInfo.TeamID == 1)
                {//Player unclaims enemy ship
                    plshipInfo.TeamID = -1;
                    plshipInfo.photonView.RPC("DoClaimRemovedVisuals", PhotonTargets.All, Array.Empty<object>());
                    if (PhotonNetwork.isMasterClient)
                    {
                        PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                        {
                        "ENEMY SHIP CLAIM REMOVED!",
                        Color.white,
                        0,
                        "SHIP"
                        });
                        return;
                    }
                }
                else if (plshipInfo.TeamID != 0)
                {//Player claims unclaimed ship
                    if (PLServer.Instance.GetPlayerFromPlayerID(PLNetworkManager.Instance.LocalPlayerID).GetClassID() != 0)
                    {//Insert code here where non-captains claiming the ship will become captain of new ship
                        int CrewID = Global.GetLowestUncrewedID();
                        plshipInfo.CaptainTargetedSpaceTargetID = -1;
                        plshipInfo.TeamID = 0;
                        plshipInfo.AutoTarget = false;
                        Global.PlayerCrewList.Add(PLNetworkManager.Instance.LocalPlayerID, CrewID); //Adds ship and player to Global.PlayerCrewList and Global.Fleet as part of crew with CrewID
                        Global.Fleet.Add(CrewID,plshipInfo.ShipID);
                        plshipInfo.SelectedActorID = "";
                        plshipInfo.IsRelicHunter = false;
                        plshipInfo.IsBountyHunter = false;
                        PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                            {
                        PLNetworkManager.Instance.LocalPlayerID,
                        Color.blue,
                        0,
                        "SHIP"
                            });
                        if (PhotonNetwork.isMasterClient)
                        {
                            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                            {
                        "SHIP CLAIMED!",
                        Color.blue,
                        0,
                        "SHIP"
                            });
                            if (plshipInfo.PersistantShipInfo != null)
                            {//Persistant ship infos are ships that are bound to the sector (If left there, they'll remain there)*
                                if (PLServer.Instance.AllPSIs.Contains(plshipInfo.PersistantShipInfo))
                                {
                                    PLServer.Instance.AllPSIs.Remove(plshipInfo.PersistantShipInfo);
                                }
                                plshipInfo.PersistantShipInfo = null;
                            }
                        }
                        if (!__instance.LongRangeCommsDisabled && PLEncounterManager.Instance.PlayerShip_WhenEnteredSector != plshipInfo)
                        {
                            plshipInfo.IsFlagged = true;
                        }
                        foreach (PLUIScreen pluiscreen in PLEncounterManager.Instance.PlayerShip.MyScreenBase.AllScreens)
                        {
                            if (pluiscreen != null)
                            {
                                pluiscreen.PlayerControlAlpha = 1f;
                            }
                        }
                        if (PhotonNetwork.isMasterClient)
                        {
                            __instance.photonView.RPC("ClaimShip", PhotonTargets.Others, new object[]
                            {
                        inShipID
                            });
                        }
                    }
                    else
                    {
                        PulsarPluginLoader.Utilities.Messaging.Echo(PLServer.Instance.GetPlayerFromPlayerID(PLNetworkManager.Instance.LocalPlayerID), "You're a captain!");
                    }
                }
            }
        }
    }
}
/* Notes:
 * If we implement player-ship unclaiming we can use: 
 * PLEncounterManager.Instance.PlayerShip.PersistantShipInfo = new PLPersistantShipInfo(PLEncounterManager.Instance.PlayerShip.ShipTypeID, PLEncounterManager.Instance.PlayerShip.FactionID, PLServer.GetCurrentSector(), 0, false, PLEncounterManager.Instance.PlayerShip.IsFlagged, false, -1, -1);
   PLEncounterManager.Instance.PlayerShip.PersistantShipInfo.EnsureNoCrew = true;
   PLEncounterManager.Instance.PlayerShip.PersistantShipInfo.ShipName = PLEncounterManager.Instance.PlayerShip.ShipNameValue;
   PLServer.Instance.AllPSIs.Add(PLEncounterManager.Instance.PlayerShip.PersistantShipInfo);
 * To ensure it remains in the sector after unclaimed
 *
 * Add in a captaining feature if they click claim on the unclaimed ship
*/
