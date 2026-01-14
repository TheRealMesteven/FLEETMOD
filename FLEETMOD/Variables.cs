using PulsarModLoader;
using PulsarModLoader.CustomGUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LocomotionTeleport;
using static UnityEngine.GUILayout;

namespace FLEETMOD
{
    internal class Variables
    {
        public static bool isrunningmod = false;
        public static bool shipfriendlyfire = false;
        public static bool shipgodmode = false;
        public static int shipcount = 5;
        public static bool recentfriendlyfire = false;
        public static bool DialogGenerated = false;
        public static bool FuelDialog = false;
        public static bool CargoMenu = false;
        public static Dictionary<int, List<int>> Fleet; /*ShipID, List<PlayerID>*/
        public static List<int> Modded; // PlayerID of the Players who have Fleetmod active and running
        public static List<int> NonModded; // PlayerID of the Players who dont have Fleetmod active and running
        public static Dictionary<int /*PlayerID*/ , /*Bonus*/ int> survivalBonusDict; 
        // Dictionary that stores <playerID,healthBonus> on hostside, then it's being sent to clients
        public static int MySurvivalBonus;
        // variable for storing localplayer's healthBonus
        public static Dictionary<int /*PlayerID*/ , /*ShipID*/ int> UnModdedCrews;
        // Dictionary that stores <playerID,shipID> on host side, to teleport unmodded crews to correct ship.

        public static void ChangeShip (int PlayerID, int ShipID, int ClassID = -1)
        {
            PLPlayer pLPlayer = PLServer.Instance.GetPlayerFromPlayerID(PlayerID);
            Fleet[pLPlayer.GetPhotonPlayer().GetScore()].Remove(PlayerID);
            Fleet[ShipID].Add(PlayerID);
            if (ClassID != -1) pLPlayer.SetClassID(ClassID);
            pLPlayer.GetPhotonPlayer().SetScore(ShipID);
            ModMessages.ServerUpdateVariables.UpdateClients();
        }
        public static int GetShipCaptain (int inShipID)
        {
            foreach (PLPlayer Player in PLServer.Instance.AllPlayers)
            {
                if (Player.IsBot) continue;
                if (Player != null && Player.TeamID == 0 && Player.GetPhotonPlayer().GetScore() == inShipID && Player.GetClassID() == 0)
                        return Player.GetPlayerID();
            }
            return -1;
        }
        public static bool ShipHasCaptain (int inShipID)
        {
            if (PLServer.Instance != null && PLEncounterManager.Instance.GetShipFromID(inShipID) != null)
            {
                if (GetShipCaptain(inShipID) != -1)
                {
                    return true;
                }
            }
            return false;
        }
        public static void ReCalculateMaxPlayers()
        {
            if (Fleet.Count() * 5 != PhotonNetwork.room.MaxPlayers)
            {
                PhotonNetwork.room.MaxPlayers = Fleet.Count() * 5;
            }
            //Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, "[SHIP COUNT CHANGE] - Update Mod Message");
            ModMessages.ServerUpdateVariables.UpdateClients();
        }
#if DEBUG
        internal class Config : ModSettingsMenu
        {
            int focusedShip = -1;
            public override string Name() => "Fleetmod Debug";
            public override void Draw()
            {
                BeginHorizontal();
                Label("Running Mod: " + isrunningmod);
                Label("Friendly Fire: " + shipfriendlyfire);
                Label("Godmode: " + shipgodmode);
                EndHorizontal();
                FlexibleSpace();
                foreach (var Ship in Fleet)
                {
                    PLShipInfoBase shipInfoBase = PLEncounterManager.Instance.GetShipFromID(Ship.Key);
                    if (shipInfoBase != null)
                    {
                        Label($"Fleet Contains: [{Ship.Key}] {shipInfoBase.ShipNameValue}");
                        BeginHorizontal();
                        foreach (int i in Ship.Value)
                        {
                            PLPlayer pLPlayer = PLServer.Instance.GetPlayerFromPlayerID(i);
                            if (pLPlayer != null)
                            {
                                Label($"- [{i}] {pLPlayer.GetPlayerName()}");
                            }
                            else
                            {
                                Label($"- [{i}] n/a");
                            }
                        }
                        EndHorizontal();
                    }
                    else
                    {
                        Label($"Fleet Contains: [{Ship.Key}] n/a");
                    }
                }
                FlexibleSpace();
                BeginHorizontal();
                BeginVertical();
                Label("Modded Players");
                foreach (int k in Modded)
                {
                    BeginHorizontal();
                    Label($"id:{k}");
                    PLPlayer pLPlayer = PLServer.Instance.GetPlayerFromPlayerID(k);
                    if (pLPlayer != null)
                    {
                        Label($" name:{pLPlayer.GetPlayerName()}");
                        int score = pLPlayer.GetPhotonPlayer().GetScore();
                        Label($" spawnid:{score}");
                        PLShipInfoBase shipInfoBase2 = PLEncounterManager.Instance.GetShipFromID(score);
                        if (shipInfoBase2 != null)
                        {
                            Label($" spawnship:{shipInfoBase2.ShipNameValue}");
                        }
                        if (Button("Create FleetShip"))
                        {
                            ModMessage.SendRPC(Mod.harmonyIden, "FLEETMOD.ModMessages.ServerCreateShip", PhotonTargets.MasterClient, new object[]
                                    {
                        0,
                        k,
                        PLServer.Instance.CUShipNameGenerator.GetName(UnityEngine.Random.Range(0, 7000))
                                    });
                        }
                    }
                    EndHorizontal();
                }
                EndVertical();
                BeginVertical();
                Label("Non-Modded Players");
                foreach (int k in NonModded)
                {
                    BeginHorizontal();
                    Label($"id:{k}");
                    PLPlayer pLPlayer = PLServer.Instance.GetPlayerFromPlayerID(k);
                    if (pLPlayer != null)
                    {
                        Label($" name:{pLPlayer.GetPlayerName()}");
                        if (UnModdedCrews.TryGetValue(k, out int ShipID))
                        {
                            Label($" shipid:{ShipID}");
                            PLShipInfoBase shipInfoBase = PLEncounterManager.Instance.GetShipFromID(ShipID);
                            if (shipInfoBase != null)
                            {
                                Label($" ship:{shipInfoBase.ShipNameValue}");
                            }
                        }
                        int score = pLPlayer.GetPhotonPlayer().GetScore();
                        Label($" spawnid:{score}");
                        PLShipInfoBase shipInfoBase2 = PLEncounterManager.Instance.GetShipFromID(score);
                        if (shipInfoBase2 != null)
                        {
                            Label($" spawnship:{shipInfoBase2.ShipNameValue}");
                        }
                        if (Button("Create FleetShip"))
                        {
                            ModMessage.SendRPC(Mod.harmonyIden, "FLEETMOD.ModMessages.ServerCreateShip", PhotonTargets.MasterClient, new object[]
                                    {
                        0,
                        k,
                        PLServer.Instance.CUShipNameGenerator.GetName(UnityEngine.Random.Range(0, 7000))
                                    });
                        }
                    }
                    EndHorizontal();
                }
                EndVertical();
                EndHorizontal();
            }
            public override void OnOpen()
            {
                focusedShip = -1;
            }
        }
#endif
    }
}
