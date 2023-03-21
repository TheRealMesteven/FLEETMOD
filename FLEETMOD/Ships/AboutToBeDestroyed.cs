using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Ships
{
    [HarmonyPatch(typeof(PLShipInfoBase), "AboutToBeDestroyed")]
    internal class AboutToBeDestroyed
    {
        /// <summary>
        /// If Admiral ship is destroyed, finds new Admiral ship.
        /// If non-Admiral ship is destroyed, moves the crew to Admiral ship.
        /// If All ships dead (Server End etc), ends game.
        /// </summary>
        public static bool Prefix(PLShipInfoBase __instance)
        {
            if (!MyVariables.isrunningmod) return true;
            if (!__instance.HasBeenDestroyed && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
            {
                if (PhotonNetwork.isMasterClient && __instance.GetIsPlayerShip())
                {
                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                    {
                            "The " + __instance.ShipNameValue + " Destroyed!",
                            Color.green,
                            0,
                            "SHIP"
                    });
                }
                if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip != __instance as PLShipInfo)
                {
                    List<int> OldShipCrew = new List<int>();
                    foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                    {
                        if (plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot && plplayer.StartingShip == __instance)
                        {
                            if (plplayer.GetClassID() == 0)
                            {
                                plplayer.SetClassID(1);
                            }
                            plplayer.GetPhotonPlayer().SetScore(PhotonNetwork.player.GetScore());
                            OldShipCrew.Add(plplayer.GetPlayerID());
                            plplayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
                        }
                    }
                    MyVariables.Fleet[PhotonNetwork.player.GetScore()].AddRange(OldShipCrew);
                    MyVariables.Fleet.Remove(__instance.ShipID);
                }
                if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip == __instance as PLShipInfo)
                {
                    PulsarModLoader.Utilities.Logger.Info($"[FMDS] Fleetmod Admiral Ship Destroyed");
                    PLShipInfo NewAdmiralShip = null;
                    foreach (int FleetID in MyVariables.Fleet.Keys)
                    {
                        PLShipInfo Fleetship = (PLShipInfo)PLEncounterManager.Instance.GetShipFromID(FleetID);
                        if (Fleetship != null && !Fleetship.GetHasBeenDestroyed() && Fleetship != __instance)
                        {
                            PulsarModLoader.Utilities.Logger.Info($"[FMDS] New Admiral Ship Found!");
                            NewAdmiralShip = Fleetship;
                            break;
                        }
                    }

                    if (NewAdmiralShip != null)
                    {
                        NewAdmiralShip.TeamID = 0;
                        List<int> OldShipCrew = new List<int>();
                        foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                        {
                            if (plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.IsBot && (plplayer.StartingShip == __instance || (plplayer.StartingShip == NewAdmiralShip && plplayer.GetClassID() == 0)))
                            {
                                if (!plplayer.GetPhotonPlayer().IsMasterClient)
                                {
                                    if (plplayer.GetClassID() == 0)
                                    {
                                        plplayer.SetClassID(1);
                                    }
                                    OldShipCrew.Add(plplayer.GetPlayerID());
                                }
                                plplayer.GetPhotonPlayer().SetScore(NewAdmiralShip.ShipID);
                                plplayer.StartingShip = NewAdmiralShip;
                            }
                        }
                        MyVariables.Fleet[PhotonNetwork.player.GetScore()].AddRange(OldShipCrew);
                    }
                    MyVariables.Fleet.Remove(__instance.ShipID);
                    __instance.HasBeenDestroyed = true;
                }
                __instance.TagID = -1;
            }
            return true;
        }
    }
}

