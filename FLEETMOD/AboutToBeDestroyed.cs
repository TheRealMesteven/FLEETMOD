using System;
using HarmonyLib;
using UnityEngine;
/*
 * Checks if ship instance has been destroyed
 * On destroy, move crew of destroyed ship to host's ship/admiral ship
 * (WIP) Code for setting another ship in fleet to the Admiral ship
 * 
 */
namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLShipInfoBase), "AboutToBeDestroyed")]
    internal class AboutToBeDestroyed
    {
        public static bool Prefix(PLShipInfoBase __instance)
        {
            if (!MyVariables.isrunningmod)
            {
                return true;
            }
            else
            {
                /*continue if following evaluates true
				 *&&
				 *Ship has NOT been destroyed
				 *Server Instance != null
				 *Local Player != null
				 *Server GameHasStarted
				 *Local Player Has Started
				*/
                if (!__instance.HasBeenDestroyed && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                {
                    /*continue if following evaluates true
					 *&&
					 *this Ship is part of the Fleet
					 *Local Player is MasterClient
					*/
                    if (__instance.TagID == -23 && PhotonNetwork.isMasterClient)
                    {
                        PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                        {
                            "The " + __instance.ShipNameValue + " Destroyed!",
                            Color.green,
                            0,
                            "SHIP"
                        });
                    }
                    /*continue if following evaluates true
					 *&&
					 *Local Player is Master Client
					 *Local Player Ship != this Ship
					*/
                    if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip != __instance as PLShipInfo)
                    {
                        /*foreach player on the server
						 *continue if following evaluates true
						 *&&
						 *player != null
						 *player photonplayer != null
						 *player != masterclient
						 *player != bot
						 *player ship != this ship
						 *<summary>
						 *Moves this ship crew to Host ship
						 *</summary>
						*/
                        foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                        {
                            if (plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot && plplayer.StartingShip == __instance)
                            {
                                plplayer.SetClassID(1);
                                plplayer.GetPhotonPlayer().SetScore(PhotonNetwork.player.GetScore());
                                plplayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
                            }
                        }
                    }
                    /*continue if following evaluates true
					 *&&
					 *Local Player is Master Client
					 *Local Player Ship == this Ship
					*/
                    if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip == __instance as PLShipInfo)
                    {
                        PulsarModLoader.Utilities.Logger.Info($"[FMDS] Fleetmod Admiral Ship Destroyed");
                        PLShipInfo NewAdmiralShip = null;
                        foreach (PLShipInfo Fleetship in PLEncounterManager.Instance.AllShips.Values)
                        {
                            if (Fleetship != null && Fleetship.TagID == -23 && Fleetship != __instance)
                            {
                                PulsarModLoader.Utilities.Logger.Info($"[FMDS] New Admiral Ship Found!");
                                NewAdmiralShip = Fleetship;
                                break;
                            }
                        }
                        
                        if (NewAdmiralShip != null)
                        {
                            /* The Admiral ship is the only player ship with TeamID of 0 */
                            NewAdmiralShip.TeamID = 0;
                            /*foreach player on the server
						    *continue if following evaluates true
						    *&&
    						 *player != null
	    					 *player photonplayer != null
		    				 *player != bot
                             **||
                             **player startingship == destroyed ship
                             ***&&
                             ***player startingship == new admiral ship
                             ***player is Captain
	    					 *<summary>
		    				 *Moves the Host crew to other ship
			    			 *</summary>
				    		*/
                            foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                            {
                                if (plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.IsBot && (plplayer.StartingShip == __instance || (plplayer.StartingShip == NewAdmiralShip && plplayer.GetClassID() == 0)))
                                {
                                    if (!plplayer.GetPhotonPlayer().IsMasterClient)
                                    {
                                        plplayer.SetClassID(1);
                                    }
                                    plplayer.GetPhotonPlayer().SetScore(NewAdmiralShip.ShipID);
                                    plplayer.StartingShip = NewAdmiralShip;
                                }
                            }
                        }
                        else
                        {
                            /* Forces host to close the game and thus ends the session. */
                            PLUIEscapeMenu.Instance.OnClick_Disconnect();
                        }
                        __instance.HasBeenDestroyed = true;
                    }
                    /*continue if following evaluates true
					 *Local Player is Master Client
					 *Local Player != null
					 *Local Player Current Ship == Fleet Ship
					 *Local Player Current Ship == this ship
					 *Local Player Main Ship != this ship
					*/
                    /*
                    if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPawn() != null && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.TagID == -23 && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip == __instance && PLNetworkManager.Instance.LocalPlayer.StartingShip != __instance as PLShipInfo)
                    {
                        PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                        {
                            PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
                            0
                        });
                    }
                    */
                    __instance.TagID = -1;
                }
                return true;
            }
        }
    }
}

