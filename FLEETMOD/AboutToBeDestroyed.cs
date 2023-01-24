using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
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
                if (__instance.TagID == -23 && PhotonNetwork.isMasterClient)
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
                    foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                    {
                        if (plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.GetPhotonPlayer().IsMasterClient && !plplayer.IsBot && plplayer.StartingShip == __instance)
                        {
                            plplayer.SetClassID(1);
                            plplayer.GetPhotonPlayer().SetScore(PhotonNetwork.player.GetScore());
                            plplayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
                        }
                    }
                }
                if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer.StartingShip == __instance as PLShipInfo)
                {
                    PulsarModLoader.Utilities.Logger.Info($"[FMDS] Fleetmod Admiral Ship Destroyed");
                    PLShipInfo NewAdmiralShip = null;
                    foreach (PLShipInfo Fleetship in PLEncounterManager.Instance.AllShips.Values)
                    {
                        if (Fleetship != null && !Fleetship.HasBeenDestroyed && Fleetship.TagID == -23 && Fleetship != __instance)
                        {
                            PulsarModLoader.Utilities.Logger.Info($"[FMDS] New Admiral Ship Found!");
                            NewAdmiralShip = Fleetship;
                            break;
                        }
                    }

                    if (NewAdmiralShip != null)
                    {
                        NewAdmiralShip.TeamID = 0;
                        foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                        {
                            if (plplayer != null && plplayer.GetPhotonPlayer() != null && !plplayer.IsBot && (plplayer.StartingShip == __instance || (plplayer.StartingShip == NewAdmiralShip && plplayer.GetClassID() == 0)))
                            {
                                if (!plplayer.GetPhotonPlayer().IsMasterClient)
                                {
                                    plplayer.SetClassID(1);
                                }
                                plplayer.GetPhotonPlayer().SetScore(NewAdmiralShip.ShipID);
                                plplayer.StartingShip = NewAdmiralShip;
                            }
                        }
                    }
                    else
                    {
                        /* Forces host to close the game and thus ends the session. */
                        PLUIEscapeMenu.Instance.OnClick_Disconnect();
                    }
                    __instance.HasBeenDestroyed = true;
                }
                __instance.TagID = -1;
            }
            return true;
        }
    }
}

