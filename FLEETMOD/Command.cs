using System;
using PulsarPluginLoader.Chat.Commands;
using UnityEngine;

namespace FLEETMOD
{
    // Token: 0x02000002 RID: 2
    internal class Command
    {
        // Token: 0x02000005 RID: 5
        private class FLEETMODGodmode : IChatCommand
        {
            // Token: 0x0600000C RID: 12 RVA: 0x000029DC File Offset: 0x00000BDC
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "godmode"
                };
            }

            // Token: 0x0600000D RID: 13 RVA: 0x000029FC File Offset: 0x00000BFC
            public string Description()
            {
                return "Fleetmod Godmode";
            }

            // Token: 0x0600000E RID: 14 RVA: 0x00002A14 File Offset: 0x00000C14
            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }

            // Token: 0x0600000F RID: 15 RVA: 0x00002A38 File Offset: 0x00000C38
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
            // Token: 0x0600000C RID: 12 RVA: 0x000029DC File Offset: 0x00000BDC
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "friendlyfire"
                };
            }

            // Token: 0x0600000D RID: 13 RVA: 0x000029FC File Offset: 0x00000BFC
            public string Description()
            {
                return "Fleetmod ship friendly fire";
            }

            // Token: 0x0600000E RID: 14 RVA: 0x00002A14 File Offset: 0x00000C14
            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }

            // Token: 0x0600000F RID: 15 RVA: 0x00002A38 File Offset: 0x00000C38
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
        private class FLEETMODShipLimit : IChatCommand
        {
            // Token: 0x0600000C RID: 12 RVA: 0x000029DC File Offset: 0x00000BDC
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "shiplimit"
                };
            }

            // Token: 0x0600000D RID: 13 RVA: 0x000029FC File Offset: 0x00000BFC
            public string Description()
            {
                return "Fleetmod Ship Friendlyfire limit";
            }

            // Token: 0x0600000E RID: 14 RVA: 0x00002A14 File Offset: 0x00000C14
            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }

            // Token: 0x0600000F RID: 15 RVA: 0x00002A38 File Offset: 0x00000C38
            public bool Execute(string arguments)
            {
                if (MyVariables.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        MyVariables.shipcount = arguments[0];
                        PulsarPluginLoader.Utilities.Messaging.Notification("FLEETMOD | Ship Limit Set To:"+MyVariables.shipcount+"\nRemember -1 removes the limit");
                    }
                }
                return false;
            }
        }
    }
}