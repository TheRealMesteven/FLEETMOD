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
                            foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                            {
                                if (plplayer != null && plplayer.GetClassID() == 0 && PLServer.Instance.GetPlayerFromPlayerID(plplayer.GetPlayerID()).GetPhotonPlayer() != null)
                                {
                                    if (PLServer.Instance.GetPlayerFromPlayerID(plplayer.GetPlayerID()).GetPhotonPlayer().GetScore() == displayShipRightInfo.Ship.ShipID) // If Captain And Owns This Ship
                                    {
                                        displayShipRightInfo.TitleLabel.enabled = true;
                                        PLGlobal.SafeLabelSetText(displayShipRightInfo.TitleLabel, "ADMIRAL • " + plplayer.GetPlayerName(false).Substring(plplayer.GetPlayerName(false).LastIndexOf("•") + 2));
                                        displayShipRightInfo.TitleLabel.color = Color.yellow;
                                    }
                                }
                            }
                        }
						bool flag3 = displayShipRightInfo.Ship.GetIsPlayerShip() && displayShipRightInfo.Ship.ShipID != PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore();
                        if (flag3)
                        {
                            displayShipRightInfo.NameLabel.color = Color.green;
                            foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                            {
                                if (plplayer != null && plplayer.GetClassID() == 0 && PLServer.Instance.GetPlayerFromPlayerID(plplayer.GetPlayerID()).GetPhotonPlayer() != null)
                                {
                                    if (PLServer.Instance.GetPlayerFromPlayerID(plplayer.GetPlayerID()).GetPhotonPlayer().GetScore() == displayShipRightInfo.Ship.ShipID) // If Captain And Owns This Ship
                                    {
                                        displayShipRightInfo.TitleLabel.enabled = true;
                                        PLGlobal.SafeLabelSetText(displayShipRightInfo.TitleLabel, "CAPTAIN • " + plplayer.GetPlayerName(false).Substring(plplayer.GetPlayerName(false).LastIndexOf("•") + 2));
                                        displayShipRightInfo.TitleLabel.color = Color.green;
                                    }
                                }
                            }
                        }
                        if (displayShipRightInfo.Ship.GetIsPlayerShip() && displayShipRightInfo.TitleLabel.enabled && displayShipRightInfo.NameLabel.GetComponent<RectTransform>().anchoredPosition3D.y == 0f)
                        {
                            displayShipRightInfo.TitleLabel.alignment = TextAnchor.MiddleRight;
                            displayShipRightInfo.NameLabel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(displayShipRightInfo.NameLabel.transform.localPosition.x, -5f, displayShipRightInfo.NameLabel.transform.localPosition.z);
                            displayShipRightInfo.TitleLabel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(displayShipRightInfo.TitleLabel.transform.localPosition.x, 15f, displayShipRightInfo.TitleLabel.transform.localPosition.z);
                        }
                    }
				}
			}
		}
	}
}
