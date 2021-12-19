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
				if (PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName != "locked" && !PLNetworkManager.Instance.LocalPlayer.StartingShip.InWarp)
				{
					if ((PLEncounterManager.Instance.PlayerShip == PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID) || !PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).GetIsPlayerShip() || (PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).GetIsPlayerShip() && MyVariables.shipfriendlyfire)) && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
					{
                        PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID = target.SpaceTargetID;
                        PLEncounterManager.Instance.PlayerShip.LastCaptainTargetedShipIDLocallyChangedTime = Time.time;
                        if (!PhotonNetwork.isMasterClient)
                        {
                            PLEncounterManager.Instance.PlayerShip.photonView.RPC("Captain_SetTargetShip", PhotonTargets.MasterClient, new object[] 
                            {
                            PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID
                            });
                        }
                    }
					if ((PLEncounterManager.Instance.GetShipFromID(target.SpaceTargetID).GetIsPlayerShip() && !PLNetworkManager.Instance.LocalPlayer.OnPlanet && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.MyShipControl.ShipInfo.GetCurrentShipControllerPlayerID() != PLNetworkManager.Instance.LocalPlayer.GetPlayerID()) && !MyVariables.shipfriendlyfire)
					{
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
