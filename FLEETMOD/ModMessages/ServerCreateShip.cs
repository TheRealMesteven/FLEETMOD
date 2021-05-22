using System;
using PulsarPluginLoader;
using UnityEngine;

namespace FLEETMOD.ModMessages
{
    class ServerCreateShip : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
            {
                if (Global.shiplimit != 0 && (Global.shiplimit * 5) != PhotonNetwork.room.MaxPlayers && (Global.shiplimit * 5) > PhotonNetwork.room.MaxPlayers) // if limit != 0 & limit != count & limit > count
                {
                    PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID((int)arguments[1]);
                    int CrewID = Global.GetLowestUncrewedID();
                    //int CrewID = 1; //Required to force spawning with CrewID erroring.
                    GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLGlobal.Instance.PlayerShipNetworkPrefabNames[(int)arguments[0]], new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
                    PLShipInfo shipinfo = gameObject.GetComponent<PLShipInfo>();
                    shipinfo.SetShipID(PLServer.ServerSpaceTargetIDCounter++);
                    shipinfo.AutoTarget = false;
                    shipinfo.TagID = -23; // Want to replace but is required for Hostile ship patch & Not Responding Check
                    shipinfo.TeamID = 1;
                    shipinfo.OnIsNewStartingShip();
                    shipinfo.ShipNameValue = (string)arguments[2];
                    shipinfo.LastAIAutoYellowAlertSetupTime = Time.time;
                    shipinfo.SetupShipStats(false, true);
                    //Between these 2 lines, code errors and stops continuation
                    Global.PlayerCrewList.Add((int)arguments[1], CrewID); //Adds ship and player to Global.PlayerCrewList and Global.Fleet as part of crew with CrewID
                    Global.Fleet.Add(CrewID,shipinfo);
                    //end of 2 lines
                    playerFromPlayerID.SetClassID(0);
                    //playerFromPlayerID.StartingShip = shipinfo;
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                    {
                        "The " + (string)arguments[2] + " Has Joined!",
                        Color.green,
                        0,
                        "SHIP"
                    });
                    if (Global.GetFleetShipCount() > 1)
                    {//Required to update all of the current players with the new ship
                        PulsarPluginLoader.Utilities.Logger.Info("UPDATING");
                        PulsarPluginLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.SyncCrewIDs", PhotonTargets.Others, new object[] {
                        Global.Fleet,
                        Global.PlayerCrewList
                        });
                    }//
                }
                else
                {
                    PLNetworkManager.Instance.ConsoleText.Insert(0, "Sorry " + (PLServer.Instance.GetPlayerFromPlayerID((int)arguments[1]).GetPlayerName(false)).ToString() + " and the " + (String)arguments[2] + " the Ship Limit has been reached!");
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PLServer.Instance.GetPlayerFromPlayerID((int)(arguments[1])).GetPhotonPlayer(), new object[]
                    {
                        "Ship Limit Has Been Reached!",
                        Color.red,
                        0,
                        "SHIP"
                    });
                }
            }
        }
    }
}
/* Notes:
 * <> WITH DEFAULT CREWID 1 <>
 * Ship spawns with bot crew, friendly and with shields up
*/