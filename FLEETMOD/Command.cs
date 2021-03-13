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
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && Debug && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
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
                        if (MyVariables.shipfriendlyfire) {
                            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] {
                                "SHIP FRIENDLYFIRE DISABLED", Color.white, 2, ""
                            });
                            PLEncounterManager.Instance.PlayerShip.photonView.RPC("Captain_SetTargetShip", PhotonTargets.All, new object[] // Apply Targetting
						    {
                                PLEncounterManager.Instance.PlayerShip.CaptainTargetedSpaceTargetID
                            });
                        }
                        else { PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] { "SHIP FRIENDLYFIRE ENABLED", Color.white, 2, "" }); }
                    }
                }
                return false;
            }
        }
        private class FLEETMODShipLimit : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "shiplimit"
                };
            }

            public string Description()
            {
                return "Fleetmod Ship limit";
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
                        MyVariables.shipcount = Int32.Parse(arguments);
                        PLNetworkManager.Instance.ConsoleText.Insert(0, "Ship Limit Set To " + MyVariables.shipcount);
                        PulsarPluginLoader.Utilities.Messaging.Notification("FLEETMOD | Ship Limit Set To:"+MyVariables.shipcount+"\nRemember -1 removes the limit");
                    }
                }
                return false;
            }
        }
        private class FLEETMODlog : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "log"
                };
            }

            public string Description()
            {
                return "Fleetmod debug variable chech";
            }

            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }

            public bool Execute(string arguments)
            {
                if (MyVariables.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && Debug && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        try
                        {
                            PLNetworkManager.Instance.ConsoleText.Insert(0, "Warp Target: " + PLEncounterManager.Instance.PlayerShip.WarpTargetID);
                        }
                        catch
                        {
                            PLNetworkManager.Instance.ConsoleText.Insert(0,"WARP TARGET FAIL");
                        }
                        try
                        {
                            PLNetworkManager.Instance.ConsoleText.Insert(0, "Course Goals: " + PLServer.Instance.m_ShipCourseGoals.Count);
                        }
                        catch
                        {
                            PLNetworkManager.Instance.ConsoleText.Insert(0, "COURSE GOALS FAIL");
                        }
                        try
                        {
                            PLSectorInfo map = PLStarmap.Instance.CurrentShipPath[1];
                            PLNetworkManager.Instance.ConsoleText.Insert(0, "Align To: " + map.ID);
                        }
                        catch
                        {
                            PLNetworkManager.Instance.ConsoleText.Insert(0, "ALIGN FAIL");
                        }
                    }
                }
                return false;
            }
        }
        private class FLEETMODCommandsEnable : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "fleetmoddebugon"
                };
            }

            public string Description()
            {
                return "Fleetmod Commands Enable";
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
                        if (PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Substring(PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).LastIndexOf("•") + 2) == "Mest" || PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).Substring(PLNetworkManager.Instance.LocalPlayer.GetPlayerName(false).LastIndexOf("•") + 2) == "josephlbj")
                            Debug = !Debug;
                            PLNetworkManager.Instance.ConsoleText.Insert(0, "*'* Commands "+Debug+" *'*");

                    }
                }
                return false;
            }
        }
        public static bool Debug = false;
    }
}
