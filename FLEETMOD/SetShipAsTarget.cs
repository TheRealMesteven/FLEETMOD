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
            return true; // *Broken Original disable
            if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked" && !PLNetworkManager.Instance.LocalPlayer.StartingShip.InWarp)
				{
					int captainTargetedSpaceTargetID = PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID;
					if (PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).TagID != -23 && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
					{
						PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID = target.SpaceTargetID; // Target Non Friendly as Captain
					}
                    if (PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0 && PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).TagID == -23 && !MyVariables.shipfriendlyfire)
                    {
                        PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID = target.SpaceTargetID; // Target Friendly as Captain WITH Friendly Fire
                    }
					if ((PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).TagID == -23 && !PLNetworkManager.Instance.LocalPlayer.OnPlanet && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.MyShipControl.ShipInfo.GetCurrentShipControllerPlayerID() != PLNetworkManager.Instance.LocalPlayer.GetPlayerID()) && MyVariables.shipfriendlyfire)
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
						PLTabMenu.Instance.OnClick_ClearTarget();
					}
					if (captainTargetedSpaceTargetID != PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID)
					{
						PLEncounterManager.Instance.PlayerShip.LastCaptainTargetedShipIDLocallyChangedTime = Time.time;
						PLEncounterManager.Instance.PlayerShip.photonView.RPC("Captain_SetTargetShip", PhotonTargets.MasterClient, new object[] // Apply Targetting
						{
							PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID
						});
						if (PLNetworkManager.Instance.LocalPlayer.StartingShip.TargetShip.TagID < -3 && MyVariables.shipfriendlyfire) // Clear Targetting If Target = Friendly
						{
							PLTabMenu.Instance.OnClick_ClearTarget();
						}
					}
				}
				return false;
			}
		}
	}
}
