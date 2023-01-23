using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLInGameUI), "SetShipAsTarget")]
    internal class SetShipAsTarget
    {
        public static bool Prefix(PLSpaceTarget target)
        {
            if (!MyVariables.isrunningmod)
            {
                return true;
            }
            else
            {
                /*continue if the following evaluates true
	        	         *&&
	        	         **Player Ship Instance != null
		                 **Local Player Instance != null
				 **Local Player Not in brig
		                 **Player Ship not in warp
		                 */
                if (PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer != null && /*!MyVariables.TeleportedBrig &&*/ !PLNetworkManager.Instance.LocalPlayer.StartingShip.InWarp)
                {
                    /*continue if the following evaluates true
		        	         *&&
					 **||
		        	         **New Target != Ship
					 ***||
					 ***New Target == Player Ship
					 ***New Target != Player Ship()
					 ****&&
					 ****New Target == Player Ship
					 ****Friendly Fire == True
		                	 *Local Player is Captain
			                 */
                    if ((PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID) == null || (PLEncounterManager.Instance.PlayerShip == PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID) || !PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).GetIsPlayerShip() || (PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).GetIsPlayerShip() && MyVariables.shipfriendlyfire))) && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
                    {
                        PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID = target.SpaceTargetID;
                        PLEncounterManager.Instance.PlayerShip.LastCaptainTargetedShipIDLocallyChangedTime = Time.time;
                        /*repeats for each Player in Captains Crew
						 *updates targetting to new ship
						*/
                        //foreach (PLPlayer pLPlayer in MyVariables.GetShipCrew(PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID))
                        //{
                        if (!PhotonNetwork.isMasterClient)
                            PLEncounterManager.Instance.PlayerShip.photonView.RPC("Captain_SetTargetShip", PhotonTargets.MasterClient, new object[]
                            {
                                                PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID
                            });
                        //}
                    }
                    /*continue if the following evaluates true
					 *&&
					 **&&
					 **New Target == PlayerShip
					 **Local Player NOT on planet
					 **Local Player NOT flying ship
					 *FriendlyFire != True
					*/
                    if ((PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).GetIsPlayerShip() && !PLNetworkManager.Instance.LocalPlayer.OnPlanet && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.MyShipControl.ShipInfo.GetCurrentShipControllerPlayerID() != PLNetworkManager.Instance.LocalPlayer.GetPlayerID()) && !MyVariables.shipfriendlyfire)
                    {
                        /*continue if the following evaluates true
						 *!
						 *Local Player not on New Target
						*/
                        if (!(PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).MyTLI.SubHubID == PLNetworkManager.Instance.LocalPlayer.MyCurrentTLI.SubHubID))
                        {
                            PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[] // Teleport To Friendly IF NO Friendly Fire
                	            			{
                                                PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).MyTLI.SubHubID,
                                                0
                                            });
                            PLServer.Instance.photonView.RPC("AddNotification", PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer(), new object[]
                            {
                                                "You are now aboard the " + PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).ShipNameValue,
                                                PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
                                                PLServer.Instance.GetEstimatedServerMs() + 3000,
                                                true
                            });
                        }
                        PLTabMenu.Instance.OnClick_ClearTarget();
                    }
                    /*continue if the following evaluates true
					 *Friendly Fire recently changed state
					*/
                    if (MyVariables.recentfriendlyfire)
                    {
                        PLTabMenu.Instance.OnClick_ClearTarget();
                        MyVariables.recentfriendlyfire = false;
                    }
                }
                return false;
            }
        }
    }
}
