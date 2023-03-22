using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD.ModMessages
{
	internal class ServerCreateShip : ModMessage
	{
		public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
		{
            if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
            {
                if (Variables.Fleet.Count < Variables.shipcount)
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
                    gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
                    Variables.Fleet.Add(gameObject.GetComponent<PLShipInfo>().ShipID, new List<int>());
                    Variables.ChangeShip((int)arguments[1], gameObject.GetComponent<PLShipInfo>().ShipID, 0);
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
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.MasterClient, new object[]
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