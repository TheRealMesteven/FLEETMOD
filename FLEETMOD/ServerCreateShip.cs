using System;
using PulsarPluginLoader;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000027 RID: 39
	internal class ServerCreateShip : ModMessage
	{
		// Token: 0x0600004C RID: 76 RVA: 0x000069FC File Offset: 0x00004BFC
		public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
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
	}
}
