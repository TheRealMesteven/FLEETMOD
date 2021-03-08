using System;
using PulsarPluginLoader;
using UnityEngine;

namespace FLEETMOD
{
	internal class ServerCreateShip : ModMessage
	{
		public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
		{
            if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
            {
                if (MyVariables.shipcount != 0 && (MyVariables.shipcount * 5) != PhotonNetwork.room.MaxPlayers && (MyVariables.shipcount * 5) > PhotonNetwork.room.MaxPlayers) // if limit != 0 & limit != count & limit > count
                {
                    PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID((int)arguments[1]);
                    GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLGlobal.Instance.PlayerShipNetworkPrefabNames[(int)arguments[0]], new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
                    gameObject.GetComponent<PLShipInfo>().SetShipID(PLServer.ServerSpaceTargetIDCounter++);
                    gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
                    gameObject.GetComponent<PLShipInfo>().TagID = -23;
                    gameObject.GetComponent<PLShipInfo>().TeamID = 1;
                    gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
                    gameObject.GetComponent<PLShipInfo>().ShipNameValue = (string)arguments[2];
                    gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
                    gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);
                    playerFromPlayerID.GetPhotonPlayer().SetScore(gameObject.GetComponent<PLShipInfo>().ShipID);
                    playerFromPlayerID.SetClassID(0);
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                    {
                        "The " + (string)arguments[2] + " Has Joined!",
                        Color.green,
                        0,
                        "SHIP"
                    });
                }
                else
                {
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                    {
                        "Ship Limit has been reached!",
                        Color.red,
                        0,
                        "SHIP"
                    });
                }
            }
		}
	}
}