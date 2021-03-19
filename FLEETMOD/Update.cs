using System;
using ExitGames.Client.Photon;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLServer), "Update")]
    internal class Update
    {
        public static void Postfix(PLServer __instance)
        {
            if (Global.isrunningmod)
            { // If running FLEETMOD
                if (__instance != null && __instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLEncounterManager.Instance.PlayerShip != null)
                { // If In-game
                    if (!PhotonNetwork.isMasterClient)
                    { // If Crew
                        if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F1) && PLNetworkManager.Instance.LocalPlayer.GetClassID() != 0)
                        { // F1 keybind for ship spawn

                        }
                    }
                }
            }
        }
    }
}