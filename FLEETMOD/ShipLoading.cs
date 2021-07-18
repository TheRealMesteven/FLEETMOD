using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FLEETMOD
{
    internal class ShipLoading
    {
        public static void LoadShip(int CrewID, bool Stored)
        {
            PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 1");
            /// File Intended Layout
            /// Seed | FleetShip | Name | Ship Data
            if (PhotonNetwork.isMasterClient && MyVariables.isrunningmod && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 2");
                ///<summary>
                /// Below lines spawn ships from FleetConfig based on the line number
                ///</summary>
                string[] FileReadout = null;
                try
                {
                    FileReadout = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt"));
                }
                catch
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FLEETMOD] UNABLE TO LOAD FLEETCONFIG");
                }
                PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 3");
                if (FileReadout != null)
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 4");
                    int num = 0;
                    foreach (string FileLines in FileReadout)
                    {
                        PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 5");
                        if (FileLines.Length > 4)
                        {
                            PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 6");
                            string[] FileSplit = null;
                            try { FileSplit = FileLines.Split('|'); }
                            catch { }
                            PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 7");
                            if (FileSplit != null && !Stored && FileSplit[0] == PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] == "FleetShip")
                            {
                                PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 8");
                                if (CrewID == num)
                                {
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 9");
                                    PLEncounterManager.ShipLayout shipLayout = new PLEncounterManager.ShipLayout(FileSplit[3] + "," + PLEncounterManager.Instance.PlayerShip.MyStats.CreateCrewString());
                                    GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLPersistantEncounterInstance.GetPrefabNameForShipType(shipLayout.ShipType), new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
                                    gameObject.GetComponent<PLShipInfo>().SetShipID(PLServer.ServerSpaceTargetIDCounter++);
                                    gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
                                    gameObject.GetComponent<PLShipInfo>().TagID = -23;
                                    gameObject.GetComponent<PLShipInfo>().TeamID = 1;
                                    gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
                                    gameObject.GetComponent<PLShipInfo>().ShipNameValue = FileSplit[2];
                                    gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
                                    gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);
                                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                                    {
                                    "The " + FileSplit[2] + " Has Joined!",
                                    Color.green,
                                    0,
                                    "SHIP"
                                    });
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 10");
                                }

                            }
                            PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 11");
                            if (FileSplit != null && Stored && FileSplit[0] == PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] == "StoredShip")
                            {
                                PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 12");
                                if (CrewID == num)
                                {
                                    PLEncounterManager.ShipLayout shipLayout = new PLEncounterManager.ShipLayout(FileSplit[3]+","+PLEncounterManager.Instance.PlayerShip.MyStats.CreateCrewString());
                                    GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLPersistantEncounterInstance.GetPrefabNameForShipType(shipLayout.ShipType), new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13c");
                                    gameObject.GetComponent<PLShipInfo>().SetShipID(PLServer.ServerSpaceTargetIDCounter++);
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13d");
                                    gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13e");
                                    gameObject.GetComponent<PLShipInfo>().TagID = -23;
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13f");
                                    gameObject.GetComponent<PLShipInfo>().TeamID = 1;
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13g");
                                    gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13h");
                                    gameObject.GetComponent<PLShipInfo>().ShipNameValue = FileSplit[2];
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13i");
                                    gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13j");
                                    gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13k");
                                    gameObject.GetComponent<PLShipInfo>().MyStats.FormatToDataString(shipLayout.Data);
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 13l");
                                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                                    {
                                    "The " + FileSplit[2] + " Has Joined!",
                                    Color.green,
                                    0,
                                    "SHIP"
                                    });
                                    PulsarPluginLoader.Utilities.Logger.Info("[FM] LS 14");
                                }
                            }
                            num++;
                        }
                    }
                }
                ///
            }
        }
        public static string[] GetShipList(bool Stored)
        {
            ///<summary>
            /// Below lines return the names of the saved ships, the boolean switches between stored or currently active.
            ///</summary>
            if (PhotonNetwork.isMasterClient && MyVariables.isrunningmod && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 1");
                string[] FileReadout = null;
                List<string> ShipNames = new List<string>();
                try
                {
                    FileReadout = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt"));
                }
                catch
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FLEETMOD] UNABLE TO LOAD FLEETCONFIG");
                }
                PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 2");
                if (FileReadout != null)
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 3");
                    foreach (string FileLines in FileReadout)
                    {
                        PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 4");
                        if (FileLines.Length > 4)
                        {
                            PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 5");
                            string[] FileSplit = null;
                            try { FileSplit = FileLines.Split('|'); }
                            catch { }
                            PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 6");
                            if (FileSplit != null && !Stored && FileSplit[0] == PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] == "FleetShip")
                            {
                                PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 7");
                                ShipNames.Add(FileSplit[2]);
                            }
                            PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 8");
                            if (FileSplit != null && Stored && FileSplit[0] == PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] == "StoredShip")
                            {
                                PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 9");
                                ShipNames.Add(FileSplit[2]);
                            }
                        }
                    }
                }
                PulsarPluginLoader.Utilities.Logger.Info("[FM] GSL 10");
                return ShipNames.ToArray();
            }
            else
            {
                return null;
            }
            ///
        }
    }
    /// <summary>
    /// Below lines spawn each additional ship after spawning the main ship
    /// </summary>
    [HarmonyPatch(typeof(PLServer), "SpawnPlayerShipFromSaveData")]
    internal class FleetShipSpawning
    {
        public static void Postfix()
        {
            if (MyVariables.isrunningmod)
            {
                string[] StoredShips = ShipLoading.GetShipList(false);
                int Count = 0;
                foreach (string Ship in StoredShips)
                {
                    ShipLoading.LoadShip(Count, false);
                    Count++;
                }
            }
        }
    }
    ///
}
