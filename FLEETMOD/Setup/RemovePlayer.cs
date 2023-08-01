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
                PLServer.Instance.photonView.RPC("LogoutMessage", PhotonTargets.All, new object[]
                {
                        inPlayer.GetPlayerName(false)
                });

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
                Variables.survivalBonusDict.Remove(inPlayer.GetPlayerID());//Removing player from healthBonus dictonary on leave
                if (Variables.UnModdedCrews.ContainsKey(inPlayer.GetPlayerID()))
                {
                    Variables.UnModdedCrews.Remove(inPlayer.GetPlayerID());
                }
                if (Variables.Modded.Contains(inPlayer.GetPlayerID()))
                {
                    Variables.Modded.Remove(inPlayer.GetPlayerID());
                }
                if (Variables.NonModded.Contains(inPlayer.GetPlayerID()))
                {
                    Variables.NonModded.Remove(inPlayer.GetPlayerID());
                }
                if (!inPlayer.IsBot) Variables.Fleet[inPlayer.GetPhotonPlayer().GetScore()].Remove(inPlayer.GetPlayerID());
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
