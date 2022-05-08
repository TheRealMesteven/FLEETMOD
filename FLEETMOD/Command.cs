using System;
using PulsarModLoader;
using PulsarModLoader.Chat.Commands;
using PulsarModLoader.Chat.Commands.CommandRouter;
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
        /*public class FLEETMODChangeClass : ChatCommand
        {
            public override string[] CommandAliases()
            {
                return new string[]
                {
                    "fmcc"
                };
            }

            public override string Description()
            {
                return "Fleetmod change player class";
            }

            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0] + "[ShipID] [PlayerID] [ClassID]";
            }

            public override void Execute(string arguments)
            {
                string[] args = arguments.Split(' ');
                if (MyVariables.isrunningmod)
                {
                    PLPlayer Player = PLServer.Instance.GetPlayerFromPlayerID(Int32.Parse(args[1]));
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        MyVariables.UnModdedCrews.Add(Int32.Parse(args[1]), Int32.Parse(args[0])); // Adds to UnModded Crew dictionary (Holds PlayerID and ShipID)
                        PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[] // Assigns Class
                        {
                            Int32.Parse(args[1]),
                            Int32.Parse(args[2])
                        });
                        Player.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[] // Teleport to ship
                        {
                            PLEncounterManager.Instance.GetShipFromID(Int32.Parse(args[0])).MyTLI.SubHubID,
                            0
                        });
                        Player.StartingShip = (PLShipInfo)PLEncounterManager.Instance.GetShipFromID(Int32.Parse(args[0])); // Set Starting Ship
                        Player.GetPhotonPlayer().SetScore(Int32.Parse(args[0])); // Update Score
                        PulsarModLoader.Utilities.Messaging.Notification("Assigning Complete.", PLNetworkManager.Instance.LocalPlayer, Int32.Parse(args[1]), 4000, false);
                        PulsarModLoader.Utilities.Messaging.Notification("You have been assigned to the " + PLEncounterManager.Instance.GetShipFromID(Int32.Parse(args[0])).ShipNameValue + "!", Player, Int32.Parse(args[1]), 4000, false);
                    }
                }
                return;
            }
        }*/
        public class FLEETMODChangeClass : PublicCommand
        {
            public override string[] CommandAliases()
            {
                return new string[]
                {
                    "fmcc"
                };
            }

            public override string Description()
            {
                return "Fleetmod public command to change ship and class\n Enter the Ship name or ID (No Spaces, but partial accuracies work) then the Class Name or ID";
            }

            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0] + "[ShipID/Name] [ClassID/Name]";
            }

            public override void Execute(string arguments, int SenderID)
            {
                string[] args = arguments.Split(' '); // Args[0] = ShipName Args[1] = ClassName
                if (MyVariables.isrunningmod && PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                {
                    PLPlayer Player = PLServer.Instance.GetPlayerFromPlayerID(SenderID);
                    if (arguments.Length < 2)
                    {
                        PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, "To change class try: \n !" + this.CommandAliases()[0] + " [ShipID/Name or approximate] [ClassID/Name]");
                        return;
                    }
                    int ClassID = Player.GetClassID();
                    int ShipID = Player.GetPhotonPlayer().GetScore();
                    if (int.TryParse(args[0], out ShipID)) // ShipID is an integer
                    {
                        if (PLEncounterManager.Instance.GetShipFromID(ShipID) == null || !PLEncounterManager.Instance.GetShipFromID(ShipID).GetIsPlayerShip())
                        {
                            ShipID = Player.GetPhotonPlayer().GetScore();
                        }
                    }
                    else
                    {
                        foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values) // ShipID is not an integer
                        {
                            if (plshipInfoBase.GetIsPlayerShip() && plshipInfoBase != null)
                            {
                                if (plshipInfoBase.ShipNameValue.ToLower() == args[0].ToLower())
                                {
                                    ShipID = plshipInfoBase.ShipID;
                                    break;
                                }
                                if (plshipInfoBase.ShipNameValue.ToLower().Contains(args[0].ToLower()))
                                {
                                    ShipID = plshipInfoBase.ShipID;
                                }
                            }
                        }
                    }
                    if (int.TryParse(args[1], out ClassID) && args[1].Length < 3) // ClassID is an integer
                    {
                        if (ClassID < 0 || ClassID > 4)
                        {
                            ClassID = Player.GetClassID();
                        }
                        if (ClassID == 0 && !Player.GetPlayerName(false).Contains("•"))
                        {
                            PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, "Sorry " + Player.GetPlayerName() + ", you need Fleetmod to Captain a ship!");
                            ClassID = Player.GetClassID();
                        }
                    }
                    else 
                    {
                        ClassID = Player.GetClassID();
                        switch (args[1].ToLower().Substring(0, 1)) // ClassID is not an integer
                        {
                            case "c":
                                if (!Player.GetPlayerName(false).Contains("•"))
                                {
                                    PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, "Sorry " + Player.GetPlayerName() + ", you need Fleetmod to Captain a ship!");
                                    break;
                                }
                                ClassID = 0;
                                break;
                            case "p":
                                ClassID = 1;
                                break;
                            case "s":
                                ClassID = 2;
                                break;
                            case "w":
                                ClassID = 3;
                                break;
                            case "e":
                                ClassID = 4;
                                break;
                            default:
                                break;
                        }
                    }
                    if (MyVariables.UnModdedCrews.ContainsKey(Player.GetPlayerID()))
                    {
                        MyVariables.UnModdedCrews[Player.GetPlayerID()] = ShipID;
                    }
                    else
                    {
                        MyVariables.UnModdedCrews.Add(Player.GetPlayerID(), ShipID); // Adds to UnModded Crew dictionary (Holds PlayerID and ShipID)
                    }
                    PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[] // Assigns Class
                    {
                            Player.GetPlayerID(),
                            ClassID
                    });
                        Player.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[] // Teleport to ship
                        {
                            PLEncounterManager.Instance.GetShipFromID(ShipID).MyTLI.SubHubID,
                            0
                        });
                        Player.StartingShip = (PLShipInfo)PLEncounterManager.Instance.GetShipFromID(ShipID); // Set Starting Ship
                        Player.GetPhotonPlayer().SetScore(ShipID); // Update Score
                    PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, Player.GetPlayerName() + ", you are now a " + Player.GetClassName() + " onboard the " + Player.StartingShip.ShipNameValue + "!");
                }
                return;
            }
        }
        public class FLEETMODForceChangeClass : ChatCommand
        {
            public override string[] CommandAliases()
            {
                return new string[]
                {
                    "fmcc"
                };
            }

            public override string Description()
            {
                return "Fleetmod public command to change ship and class\n Enter the Ship name or ID (No Spaces, but partial accuracies work) then the Class Name or ID";
            }

            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0] + "[ShipID/Name] [ClassID/Name]";
            }

            public override void Execute(string arguments)
            {
                string[] args = arguments.Split(' ');
                int.TryParse(args[0], out int PlayerID);
                int.TryParse(args[1], out int ClassID);
                int.TryParse(args[2], out int ShipID);
                PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[] // Assigns Class
                {
                            PlayerID,
                            ClassID
                });
                PLServer.Instance.GetPlayerFromPlayerID(PlayerID).photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[] // Teleport to ship
                {
                            PLEncounterManager.Instance.GetShipFromID(ShipID).MyTLI.SubHubID,
                            0
                });
                PLServer.Instance.GetPlayerFromPlayerID(PlayerID).StartingShip = (PLShipInfo)PLEncounterManager.Instance.GetShipFromID(ShipID); // Set Starting Ship
                PLServer.Instance.GetPlayerFromPlayerID(PlayerID).GetPhotonPlayer().SetScore(ShipID); // Update Score
                PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, PLServer.Instance.GetPlayerFromPlayerID(PlayerID).GetPlayerName() + ", you are now a " + PLServer.Instance.GetPlayerFromPlayerID(PlayerID).GetClassName() + " onboard the " + PLServer.Instance.GetPlayerFromPlayerID(PlayerID).StartingShip.ShipNameValue + "!");
            }
        }
        internal class FLEETMODBeamMeUp : PublicCommand
        {
            // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
            public override string[] CommandAliases()
            {
                return new string[]
                {
                "ship"
                };
            }

            // Token: 0x06000002 RID: 2 RVA: 0x00002070 File Offset: 0x00000270
            public override string Description()
            {
                return "Teleport the player back to the ship";
            }

            // Token: 0x06000003 RID: 3 RVA: 0x00002088 File Offset: 0x00000288
            public override void Execute(string arguments, int SenderID)
            {
                if (MyVariables.isrunningmod && PhotonNetwork.isMasterClient)
                {
                    PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(SenderID);
                    int subHubID = playerFromPlayerID.StartingShip.MyTLI.SubHubID;
                    if (playerFromPlayerID.SubHubID == subHubID)
                    {
                        PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, playerFromPlayerID.GetPlayerName() + " is already on their ship!");
                    }
                    else
                    {
                        playerFromPlayerID.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                        {
                    subHubID,
                    0
                        });
                        PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, playerFromPlayerID.GetPlayerName() + " has teleported to their ship.");
                    }
                }
            }
        }
        public class FLEETMODFriendlyFire : ChatCommand
        {
            public override string[] CommandAliases()
            {
                return new string[]
                {
                    "friendlyfire"
                };
            }

            public override string Description()
            {
                return "Fleetmod ship friendly fire";
            }

            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }

            public override void Execute(string arguments)
            {
                if (MyVariables.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        if (MyVariables.shipfriendlyfire)
                        {
                            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] { "SHIP FRIENDLYFIRE DISABLED", Color.white, 2, "" });
                            MyVariables.recentfriendlyfire = true;
                        }
                        else
                        {
                            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[] { "SHIP FRIENDLYFIRE ENABLED", Color.white, 2, "" });
                        }
                        MyVariables.shipfriendlyfire = !MyVariables.shipfriendlyfire;
                        PulsarModLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.HostUpdateVariables", PhotonTargets.All, new object[] { });
                    }
                }
                return;
            }
        }/*
        public class FLEETMODShipLimit : ChatCommand
        {
            public override string[] CommandAliases()
            {
                return new string[]
                {
                    "shiplimit"
                };
            }
            
            public override string Description()
            {
                return "Fleetmod Ship limit";
            }

            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }

            public override void Execute(string arguments)
            {
                if (MyVariables.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        MyVariables.shipcount = Int32.Parse(arguments);
                        PLNetworkManager.Instance.ConsoleText.Insert(0, "Ship Limit Set To " + MyVariables.shipcount);
                        PulsarModLoader.Utilities.Messaging.Notification("FLEETMOD | Ship Limit Set To:"+MyVariables.shipcount+"\nRemember -1 removes the limit");
                    }
                }
                return;
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
                                PulsarModLoader.Utilities.Messaging.Notification("FLEETMOD | ID : " + __target + " | Name : " + plshipInfoBase.ShipName + " Is a Player Ship\n");
                                bool __detect = false;
                                try
                                {
                                    __detect = plshipInfoBase.MySensorObjectShip.IsDetectedBy(PLEncounterManager.Instance.PlayerShip);
                                }
                                catch { }
                                PulsarModLoader.Utilities.Messaging.Notification("FLEETMOD | Ship Detected : "+__detect);
                            }
                            else
                            {
                                int __target = -1;
                                try
                                {
                                    __target = plshipInfoBase.TargetSpaceTarget.SpaceTargetID;
                                }
                                catch { }
                                PulsarModLoader.Utilities.Messaging.Notification("FLEETMOD | ID : " + __target + " | Name : " + plshipInfoBase.ShipName + " Is a Hostile Ship\n");
                                bool __detect = false;
                                try
                                {
                                    __detect = PLEncounterManager.Instance.PlayerShip.MySensorObjectShip.IsDetectedBy(plshipInfoBase);
                                }
                                catch { }
                                PulsarModLoader.Utilities.Messaging.Notification("FLEETMOD | Ship Detected : " + __detect);
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
        }
        public static bool Debug = false;
    }*/
    }
}