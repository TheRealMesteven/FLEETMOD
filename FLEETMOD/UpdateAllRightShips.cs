using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLInGameUI), "UpdateAllRightShips")]
	internal class UpdateAllRightShips
	{
		public static void Postfix(List<PLInGameUI.DisplayShipRightInfo> ___AllDisplayRightShipRightInfos)
		{
            //Check if mod running
			if (MyVariables.isrunningmod)
			{
                /*continue iff the following evaluates true
                 *&&
                 **Player Ship Instance != null
                 **Server Instance != null
                 **Local Player Instance != null
                 **Game Has Started
                 **GetHasStarted returns true
                 ***(Previous two are redundant)
                 */               
				if (PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
				{
					for (int i = 0; i < ___AllDisplayRightShipRightInfos.Count; i++)
					{
						PLInGameUI.DisplayShipRightInfo displayShipRightInfo = ___AllDisplayRightShipRightInfos[i];
						if (displayShipRightInfo.Ship.ShipID == PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore())
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
                        /*continue iff the following evaluates true
                         * &&
                         **GetIsPlayerShip returns true
                         **ShipID != Player Score
                         */
                        if (displayShipRightInfo.Ship.GetIsPlayerShip() && displayShipRightInfo.Ship.ShipID != PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore())
                        {
                            displayShipRightInfo.NameLabel.color = Color.green;
                            foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                            {
                                /*continue iff the following evaluates true
                                 * &&
                                 * *plplayer != null
                                 * *player classid != 0
                                 * *player server instance != null
                                 */
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
                        /*continue iff the following evaluates true
                        * &&
                        **GetIsPlayerShip returns true
                        **Ship info title enabled
                        ** ship is upright
                        */
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
