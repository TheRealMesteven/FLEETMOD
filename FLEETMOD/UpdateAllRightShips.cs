using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x0200001B RID: 27
	[HarmonyPatch(typeof(PLInGameUI), "UpdateAllRightShips")]
	internal class UpdateAllRightShips
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00005800 File Offset: 0x00003A00
		public static void Postfix(List<PLInGameUI.DisplayShipRightInfo> ___AllDisplayRightShipRightInfos)
		{
			bool isrunningmod = MyVariables.isrunningmod;
			if (isrunningmod)
			{
				bool flag = PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted();
				if (flag)
				{
					int count = ___AllDisplayRightShipRightInfos.Count;
					for (int i = 0; i < count; i++)
					{
						PLInGameUI.DisplayShipRightInfo displayShipRightInfo = ___AllDisplayRightShipRightInfos[i];
						bool flag2 = displayShipRightInfo.Ship.ShipID == PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore();
						if (flag2)
						{
							displayShipRightInfo.NameLabel.color = Color.yellow;
						}
						bool flag3 = displayShipRightInfo.Ship.GetIsPlayerShip() && displayShipRightInfo.Ship.ShipID != PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore();
						if (flag3)
						{
							displayShipRightInfo.NameLabel.color = Color.green;
						}
					}
				}
			}
		}
	}
}
