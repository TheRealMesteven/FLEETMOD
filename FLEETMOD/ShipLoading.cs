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
            /// File Intended Layout
            /// Seed | FleetShip | Name | Ship Data
            if (PhotonNetwork.isMasterClient && MyVariables.isrunningmod && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
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
                    PulsarModLoader.Utilities.Logger.Info("[FLEETMOD] UNABLE TO LOAD FLEETCONFIG");
                }
                if (FileReadout != null)
                {
                    int num = 0;
                    string FileLineToRemove = null;
                    foreach (string FileLines in FileReadout)
                    {
                        if (FileLines.Length > 4)
                        {
                            string[] FileSplit = null;
                            try { FileSplit = FileLines.Split('|'); }
                            catch { }
                            if (FileSplit != null && !Stored && FileSplit[0] == " " && FileSplit[1] == "FleetShip")
                            {
                                PulsarModLoader.Utilities.Logger.Info("[FM] " + CrewID + " " + num);
                                if (CrewID == num)
                                {
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
                                }
                                num++;

                            }
                            if (FileSplit != null && Stored && FileSplit[0] == " " && FileSplit[1] == "StoredShip")
                            {
                                if (CrewID == num)
                                {
                                    PLEncounterManager.ShipLayout shipLayout = new PLEncounterManager.ShipLayout(FileSplit[3]+","+PLEncounterManager.Instance.PlayerShip.MyStats.CreateCrewString());
                                    GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLPersistantEncounterInstance.GetPrefabNameForShipType(shipLayout.ShipType), new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
                                    gameObject.GetComponent<PLShipInfo>().SetShipID(PLServer.ServerSpaceTargetIDCounter++);
                                    gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
                                    gameObject.GetComponent<PLShipInfo>().TagID = -23;
                                    gameObject.GetComponent<PLShipInfo>().TeamID = 1;
                                    gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
                                    gameObject.GetComponent<PLShipInfo>().ShipNameValue = FileSplit[2];
                                    gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
                                    gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);
                                    gameObject.GetComponent<PLShipInfo>().MyStats.FormatToDataString(shipLayout.Data);
                                    PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                                    {
                                    "The " + FileSplit[2] + " Has Joined!",
                                    Color.green,
                                    0,
                                    "SHIP"
                                    });
                                    FileLineToRemove = FileLines;
                                }
                                num++;
                            }
                        }
                    }
                    ///<summary>
                    /// Below lines remove the bug of stored ships not deleting.
                    ///</summary>
                    if (Stored)
                    {
                        using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt")))
                        {
                            streamWriter.WriteLine(FileReadout = FileReadout.Where(val => val != FileLineToRemove).ToArray());
                        }
                    }
                    ///
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
                string[] FileReadout = null;
                List<string> ShipNames = new List<string>();
                try
                {
                    FileReadout = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt"));
                }
                catch
                {
                    PulsarModLoader.Utilities.Logger.Info("[FLEETMOD] UNABLE TO LOAD FLEETCONFIG");
                }
                if (FileReadout != null)
                {
                    foreach (string FileLines in FileReadout)
                    {
                        if (FileLines.Length > 4)
                        {
                            string[] FileSplit = null;
                            try { FileSplit = FileLines.Split('|'); }
                            catch { }
                            if (FileSplit != null && !Stored && FileSplit[0] == " " && FileSplit[1] == "FleetShip")
                            {
                                ShipNames.Add(FileSplit[2]);
                            }
                            if (FileSplit != null && Stored && FileSplit[0] == " " && FileSplit[1] == "StoredShip")
                            {
                                ShipNames.Add(FileSplit[2]);
                            }
                        }
                    }
                }
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
    [HarmonyPatch(typeof(PLServer), "ServerCaptainStartGame")]
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
