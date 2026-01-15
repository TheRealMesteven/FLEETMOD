using System;
using ExitGames.Client.Photon.LoadBalancing;
using HarmonyLib;
using PulsarModLoader.Utilities;
using UnityEngine;

namespace FLEETMOD.Setup
{
    [HarmonyPatch(typeof(PLServer), "RemovePlayer")]
    internal class RemovePlayer
    {
        public static bool Prefix(PLServer __instance, ref PLPlayer inPlayer)
        {
            if (!Variables.isrunningmod) return true;
            if (inPlayer != null)
            {
                string name = inPlayer.GetPlayerName(false) + " ";
                if (inPlayer.StartingShip != null && Mod.Config.JoinLeaveShipNameExtension.Value)
                {
                    name = inPlayer.StartingShip.ShipNameValue + " • " + name;
                }
                if (!(Mod.Config.JoinLeaveBotMessage && inPlayer.IsBot))
                {
                    PLServer.Instance.photonView.RPC("LogoutMessage", PhotonTargets.All, new object[]
                    {
                    name
                    });
                }

                //int num = 0;
                if (!inPlayer.IsBot)
                {
                    if (inPlayer.GetPhotonPlayer().GetScore() > 0 && inPlayer.StartingShip != null && !inPlayer.StartingShip.HasBeenDestroyed && inPlayer.GetClassID() == 0)
                    {
                        PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                        {
                            $"{inPlayer.StartingShip.ShipNameValue}\n has lost her Captain",
                            Color.green,
                            0,
                            "SHIP"
                        });
                        /*foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                        {
                            if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer != inPlayer && plplayer.PlayerLifeTime > 10f && plplayer.GetPlayerName(false).Contains("•") && plplayer.GetClassID() != 0 && plplayer.GetPhotonPlayer().GetScore() == inPlayer.GetPhotonPlayer().GetScore() && !plplayer.IsBot)
                            {
                                num++;
                                PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
                                {
                                    plplayer.GetPlayerID(),
                                    0
                                });
                                PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                                {
                                    plplayer.GetPlayerName(false) + " \n is now the Captain",
                                    Color.green,
                                    0,
                                    "SHIP"
                                });
                                break;
                            }
                        }*/
                    }
                }

                /*if (PhotonNetwork.isMasterClient && num == 0 && inPlayer.StartingShip != PLNetworkManager.Instance.LocalPlayer.StartingShip && inPlayer.GetClassID() == 0)
                {
                    inPlayer.StartingShip.DestroySelf(inPlayer.StartingShip);
                    UnityEngine.Object.Destroy(inPlayer.StartingShip.gameObject);
                }*/
                PLServer.Instance.ClearPlayerData(inPlayer);
                PLServer.Instance.AllPlayers.Remove(inPlayer);
                int playerID = inPlayer.GetPlayerID();
                Variables.survivalBonusDict.Remove(playerID);//Removing player from healthBonus dictonary on leave
                int ship = -1;
                if (!inPlayer.IsBot)
                {
                    ship = inPlayer.GetPhotonPlayer().GetScore();
                    if (Variables.UnModdedCrews.ContainsKey(playerID))
                    {
                        Variables.UnModdedCrews.Remove(playerID);
                    }
                    if (Variables.Modded.Contains(playerID))
                    {
                        Variables.Modded.Remove(playerID);
                    }
                    if (Variables.NonModded.Contains(playerID))
                    {
                        Variables.NonModded.Remove(playerID);
                    }
                }
                else
                {
                    if (Variables.BotCrews.ContainsKey(playerID))
                    {
                        ship = Variables.BotCrews[playerID];
                        Variables.Fleet[ship].Remove(playerID);
                        Variables.BotCrews.Remove(playerID);
                    }
                }
                if (Variables.Fleet.ContainsKey(ship)) Variables.Fleet[ship].Remove(playerID);
                /*if (MyVariables.BriggedCrew.Contains(inPlayer.GetPlayerID()))
                {
                    MyVariables.BriggedCrew.Remove(inPlayer.GetPlayerID());
                }*/
                if (inPlayer.GetPawn() != null)
                {
                    inPlayer.GetPawn().transform.parent = null;
                    inPlayer.GetPawn().gameObject.SetActive(true);
                    PhotonNetwork.Destroy(inPlayer.GetPawn().gameObject);
                }
                PhotonNetwork.Destroy(inPlayer.gameObject);
                //Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, "[PLAYER LEAVE] - Update Mod Message");
                ModMessages.ServerUpdateVariables.UpdateClients();
            }
            return false;
        }
    }
}
