﻿using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Setup
{
    [HarmonyPatch(typeof(PLGame), "Update")]
    internal class AutoSpawnPlayers
    {
        /// <summary>
        /// This code automatically spawns the players as their roles.
        /// </summary>
        public static bool Prefix(ref float ___m_Lifetime, ref bool ___HasDoneSetup)
        {
            if (!MyVariables.isrunningmod) return true;
            ___m_Lifetime += Time.deltaTime;
            if (___m_Lifetime > 1f && !___HasDoneSetup && PLNetworkManager.Instance.LocalPlayer != null && PLEncounterManager.Instance.GetCPEI() != null)
            {
                ___HasDoneSetup = true;
            }
            if (PhotonNetwork.isMasterClient && PLNetworkManager.Instance.LocalPlayer != null)
            {
                if (PLServer.Instance.GameHasStarted && PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLNetworkManager.Instance.LocalPlayer.GetClassID() == -1)
                {
                    PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
                    {
                            PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
                            0
                    });
                }
                if (!PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayerID != -1 && PLEncounterManager.Instance.PlayerShip != null)
                {
                    PLServer.Instance.photonView.RPC("ServerCaptainStartGame", PhotonTargets.MasterClient, new object[]
                    {
                            0
                    });
                    PLNetworkManager.Instance.ConsoleText.Insert(0, "Game Has Started. You Are The Admiral.");
                    PLServer.Instance.GameHasStarted = true;
                    PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("<color=white>\n Welcome Admiral \n\n Use the Faction Manager Comms to spawn and manage ships! \n\n Use < > Keys To Manage Players in their crews. \n Hold Space And Click A Player Ship To Teleport.</color>"));
                }
                if (PLServer.Instance.GameHasStarted && !PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                {
                    PLServer.Instance.photonView.RPC("StartPlayer", PhotonTargets.MasterClient, new object[]
                    {
                            PLNetworkManager.Instance.LocalPlayer.GetPlayerID()
                    });
                }
                if (PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                {
                    PhotonNetwork.player.SetScore(PLEncounterManager.Instance.PlayerShip.ShipID);
                }
            }
            if (!PhotonNetwork.isMasterClient && ___m_Lifetime > 2f && PLEncounterManager.Instance.GetCPEI() != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted)
            {
                if (!PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLNetworkManager.Instance.LocalPlayer.PlayerLifeTime > 2f && PhotonNetwork.player.GetScore() == 0)
                {
                    PLServer.Instance.photonView.RPC("StartPlayer", PhotonTargets.MasterClient, new object[]
                    {
                            PLNetworkManager.Instance.LocalPlayer.GetPlayerID()
                    });
                    PLNetworkManager.Instance.ConsoleText.Insert(0, "You Have Joined - Press TAB Button To Choose Your Class And Ship");
                    PLNetworkManager.Instance.LocalPlayer.SetHasStarted(true);
                    PhotonNetwork.player.SetScore(1);
                }
                if (PLNetworkManager.Instance.LocalPlayer.GetClassID() == -1 && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLNetworkManager.Instance.LocalPlayer.PlayerLifeTime > 4f)
                {
                    PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
                    {
                            PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
                            1
                    });
                    PhotonNetwork.player.SetScore(PLEncounterManager.Instance.PlayerShip.ShipID);
                    PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("<color=white>\n Welcome To </color><color=blue>" + PhotonNetwork.room.Name + "</color><color=white>\n \n  Press TAB Button During Game To Choose Your Class & Crew. \n\n Use < > Keys To Select The Ship You Would Like To Join. \n \n Ask The Admiral To Create A New Ship Of Your Choice.\n\n Hold Space And Click A Player Ship To Teleport.</color>"));
                }
            }
            return false;
        }
    }
}
