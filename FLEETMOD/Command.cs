using System;
using PulsarPluginLoader.Chat.Commands;
using UnityEngine;

namespace FLEETMOD
{
    internal class Command
    {/*
        public class FLEETMODGodmode : IChatCommand
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

            public bool PublicCommand()
            {
                return false;
            }

            public bool Execute(string arguments, int SenderID)
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
        }*/
        public class FLEETMODFriendlyFire : IChatCommand
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

            public bool PublicCommand()
            {
                return false;
            }

            public bool Execute(string arguments, int SenderID)
            {
                if (MyVariables.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        MyVariables.shipfriendlyfire = !MyVariables.shipfriendlyfire;
                        if (MyVariables.shipfriendlyfire) {
                            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] {
                                "SHIP FRIENDLYFIRE DISABLED", Color.white, 2, ""
                            });
                        }
                        else {
                            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] { "SHIP FRIENDLYFIRE ENABLED", Color.white, 2, "" });
                            MyVariables.recentfriendlyfire = true;
                        }
                        PulsarPluginLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.HostUpdateVariables", PhotonTargets.All, new object[] { });
                    }
                }
                return false;
            }
        }
        public class FLEETMODShipLimit : IChatCommand
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
            
            public bool PublicCommand()
            {
                return false;
            }

            public bool Execute(string arguments, int SenderID)
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
        }/*
        public class FLEETMODShipDetect : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "shipdetection"
                };
            }

            public string Description()
            {
                return "Fleetmod ship detection";
            }

            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }

            public bool PublicCommand()
            {
                return false;
            }

            public bool Execute(string arguments, int SenderID)
            {
                if (MyVariables.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
                        {
                            if (plshipInfoBase == PLEncounterManager.Instance.PlayerShip)
                            {
                                int __target = -1;
                                try
                                {
                                    __target = plshipInfoBase.TargetSpaceTarget.SpaceTargetID;
                                }
                                catch { }
                                PulsarPluginLoader.Utilities.Messaging.Notification("FLEETMOD | ID : " + __target + " | Name : " + plshipInfoBase.ShipName + " Is a Player Ship\n");
                                bool __detect = false;
                                try
                                {
                                    __detect = plshipInfoBase.MySensorObjectShip.IsDetectedBy(PLEncounterManager.Instance.PlayerShip);
                                }
                                catch { }
                                PulsarPluginLoader.Utilities.Messaging.Notification("FLEETMOD | Ship Detected : "+__detect);
                            }
                            else
                            {
                                int __target = -1;
                                try
                                {
                                    __target = plshipInfoBase.TargetSpaceTarget.SpaceTargetID;
                                }
                                catch { }
                                PulsarPluginLoader.Utilities.Messaging.Notification("FLEETMOD | ID : " + __target + " | Name : " + plshipInfoBase.ShipName + " Is a Hostile Ship\n");
                                bool __detect = false;
                                try
                                {
                                    __detect = PLEncounterManager.Instance.PlayerShip.MySensorObjectShip.IsDetectedBy(plshipInfoBase);
                                }
                                catch { }
                                PulsarPluginLoader.Utilities.Messaging.Notification("FLEETMOD | Ship Detected : " + __detect);
                            }

                        }
                    }
                }
                return false;
            }
        }
        public class FLEETMODlog : IChatCommand
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

            public bool PublicCommand()
            {
                return false;
            }

            public bool Execute(string arguments, int SenderID)
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
        public class FLEETMODCommandsEnable : IChatCommand
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

            public bool PublicCommand()
            {
                return false;
            }

            public bool Execute(string arguments, int SenderID)
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
        }*/
        public static bool Debug = false;
    }
}
