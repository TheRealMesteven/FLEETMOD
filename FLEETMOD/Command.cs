using System;
using PulsarPluginLoader.Chat.Commands;
using UnityEngine;

namespace FLEETMOD
{
    internal class Command
    {
        private class FLEETMODGodmode : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "godmode"
                };
            }
            public string Description()
            {
                return "Fleetmod Godmode";
            }
            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }
            public bool Execute(string arguments)
            {
                if (MyVariables.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        MyVariables.shipgodmode = !MyVariables.shipgodmode;
                        if (MyVariables.shipgodmode) { PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] { "SHIP GODMODE ENABLED", Color.white, 2, "" }); }
                        else { PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] { "SHIP GODMODE DISABLED", Color.white, 2, "" }); }
                    }
                }
                return false;
            }
        }
        private class FLEETMODFriendlyFire : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "friendlyfire"
                };
            }
            public string Description()
            {
                return "Fleetmod ship friendly fire";
            }
            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }
            public bool Execute(string arguments)
            {
                if (MyVariables.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        MyVariables.shipfriendlyfire = !MyVariables.shipfriendlyfire;
                        PulsarPluginLoader.ModMessage.SendRPC("Michael+Mest.Fleetmod", "FLEETMOD.HostUpdateVariables", PhotonTargets.All, new object[]{});
                        if (MyVariables.shipfriendlyfire) { PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] { "SHIP FRIENDLYFIRE DISABLED", Color.white, 2, "" }); }
                        else { PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] { "SHIP FRIENDLYFIRE ENABLED", Color.white, 2, "" }); }
                    }
                }
                return false;
            }
        }
    }
}