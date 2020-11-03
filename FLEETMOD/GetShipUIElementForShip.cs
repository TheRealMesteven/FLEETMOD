using System;
using System.Collections.Generic;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(PLUIOutsideWorldUI), "GetShipUIElementForShip")]
	internal class GetShipUIElementForShip
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002170 File Offset: 0x00000370
		public static bool Prefix(ref List<PLUIOutsideWorldUI.ShipUIElement> ___DisplayedShipUIElements)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (PLUIOutsideWorldUI.ShipUIElement shipUIElement in ___DisplayedShipUIElements)
				{
					bool flag2 = PLEncounterManager.Instance.GetShipFromID(PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore()).ShipID == shipUIElement.Ship.ShipID && shipUIElement.Ship.GetIsPlayerShip();
					if (flag2)
					{
						shipUIElement.ShipName.text = "<color=yellow>" + shipUIElement.Ship.ShipNameValue + "</color>";
					}
					else
					{
						bool isPlayerShip = shipUIElement.Ship.GetIsPlayerShip();
						if (isPlayerShip)
						{
							shipUIElement.ShipName.text = "<color=lime>" + shipUIElement.Ship.ShipNameValue + "</color>";
						}
					}
				}
				result = true;
			}
			return result;
		}
	}
}
