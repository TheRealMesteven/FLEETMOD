using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Visuals
{
    [HarmonyPatch(typeof(PLInGameUI), "UpdateAllRightShips")]
    internal class UpdateAllRightShips
    {
        /// <summary>
        /// Updates the Top-Right UI
        /// Makes Admiral Ship name Yellow with Admiral name banner.
        /// Makes Fleet Ship name Green with Captain name banner.
        /// </summary>
        public static void Postfix(List<PLInGameUI.DisplayShipRightInfo> ___AllDisplayRightShipRightInfos)
        {
            if (!MyVariables.isrunningmod) return;
            if (PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
            {
                for (int i = 0; i < ___AllDisplayRightShipRightInfos.Count; i++)
                {
                    PLInGameUI.DisplayShipRightInfo displayShipRightInfo = ___AllDisplayRightShipRightInfos[i];
                    if (displayShipRightInfo.Ship != null && displayShipRightInfo.Ship.GetIsPlayerShip())
                    {
                        if (displayShipRightInfo.Ship.ShipID == PhotonNetwork.masterClient.GetScore())
                        {
                            displayShipRightInfo.NameLabel.color = Color.yellow;
                            PLPlayer Admiral = PLServer.GetPlayerForPhotonPlayer(PhotonNetwork.masterClient);
                            if (Admiral != null)
                            {
                                displayShipRightInfo.TitleLabel.enabled = true;
                                PLGlobal.SafeLabelSetText(displayShipRightInfo.TitleLabel, $"ADMIRAL • {Admiral.GetPlayerName(false)}");
                                displayShipRightInfo.TitleLabel.color = Color.yellow;
                            }
                            else
                            {
                                displayShipRightInfo.TitleLabel.enabled = false;
                            }
                        }
                        else
                        {
                            displayShipRightInfo.NameLabel.color = Color.green;
                            PLPlayer Captain = PLServer.Instance.GetPlayerFromPlayerID(MyVariables.GetShipCaptain(displayShipRightInfo.Ship.ShipID));
                            if (Captain != null)
                            {
                                displayShipRightInfo.TitleLabel.enabled = true;
                                PLGlobal.SafeLabelSetText(displayShipRightInfo.TitleLabel, $"CAPTAIN • {Captain.GetPlayerName(false)}");
                                displayShipRightInfo.TitleLabel.color = Color.green;
                            }
                            else
                            {
                                displayShipRightInfo.TitleLabel.enabled = false;
                            }
                        }
                        if (displayShipRightInfo.TitleLabel.enabled && displayShipRightInfo.NameLabel.GetComponent<RectTransform>().anchoredPosition3D.y == 0f)
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
