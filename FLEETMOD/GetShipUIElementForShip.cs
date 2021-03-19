using System;
using System.Collections.Generic;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLUIOutsideWorldUI), "GetShipUIElementForShip")]
	internal class GetShipUIElementForShip
	{
		public static bool Prefix(ref List<PLUIOutsideWorldUI.ShipUIElement> ___DisplayedShipUIElements)
		{
            return true; // *Broken Original disable
            if (MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				foreach (PLUIOutsideWorldUI.ShipUIElement shipUIElement in ___DisplayedShipUIElements)
				{
					if (PLEncounterManager.Instance.GetShipFromID(PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore()).ShipID == shipUIElement.Ship.ShipID && shipUIElement.Ship.GetIsPlayerShip())
					{
						shipUIElement.ShipName.text = "<color=yellow>" + shipUIElement.Ship.ShipNameValue + "</color>";
					}
					else
					{
						if (shipUIElement.Ship.GetIsPlayerShip())
						{
							shipUIElement.ShipName.text = "<color=lime>" + shipUIElement.Ship.ShipNameValue + "</color>";
						}
					}
				}
				return true;
			}
		}
	}
}
