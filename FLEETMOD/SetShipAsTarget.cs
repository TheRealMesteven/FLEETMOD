using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000015 RID: 21
	[HarmonyPatch(typeof(PLInGameUI), "SetShipAsTarget")]
	internal class SetShipAsTarget
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00004E10 File Offset: 0x00003010
		public static bool Prefix(PLSpaceTarget target)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked" && !PLNetworkManager.Instance.LocalPlayer.StartingShip.InWarp;
				if (flag2)
				{
					int captainTargetedSpaceTargetID = PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID;
					bool flag3 = PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).TagID != -23 && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0;
					if (flag3)
					{
						PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID = target.SpaceTargetID;
					}
					bool flag4 = (PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).TagID == -23 && !PLNetworkManager.Instance.LocalPlayer.OnPlanet && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.MyShipControl.ShipInfo.GetCurrentShipControllerPlayerID() != PLNetworkManager.Instance.LocalPlayer.GetPlayerID()) || PLServer.Instance.GetCurrentHubID() == 0;
					if (flag4)
					{
						PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
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
						PLTabMenu.Instance.OnClick_ClearTarget();
					}
					bool flag5 = captainTargetedSpaceTargetID != PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID;
					if (flag5)
					{
						PLEncounterManager.Instance.PlayerShip.LastCaptainTargetedShipIDLocallyChangedTime = Time.time;
						PLEncounterManager.Instance.PlayerShip.photonView.RPC("Captain_SetTargetShip", PhotonTargets.MasterClient, new object[]
						{
							PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID
						});
						bool flag6 = PLNetworkManager.Instance.LocalPlayer.StartingShip.TargetShip.TagID < -3;
						if (flag6)
						{
							PLTabMenu.Instance.OnClick_ClearTarget();
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
