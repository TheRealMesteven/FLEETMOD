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
                string[] FileReadout = null;
                try
                {
                    FileReadout = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt"));
                }
                catch
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FLEETMOD] UNABLE TO LOAD FLEETCONFIG");
                }
                if (FileReadout != null)
                {
                    int num = 0;
                    foreach (string FileLines in FileReadout)
                    {
                        string[] FileSplit = null;
                        try { FileSplit = FileLines.Split('|'); }
                        catch { }
                        if (FileSplit != null && !Stored && FileSplit[0] == PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] == "FleetShip")
                        {
                            if (CrewID == num)
                            {
                                PLEncounterManager.ShipLayout shipLayout = new PLEncounterManager.ShipLayout(FileSplit[3]);
                                GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + shipLayout.ShipType, new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
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
                        }
                        if (FileSplit != null && !Stored && FileSplit[0] == PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] == "StoredShip")
                        {
                            if (CrewID == num)
                            {
                                PLEncounterManager.ShipLayout shipLayout = new PLEncounterManager.ShipLayout(FileSplit[3]);
                                GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + shipLayout.ShipType, new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
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
                        }
                        num++;
                    }
                }
            }
        }
        public static string[] GetShipList(bool Stored)
        {
            if (PhotonNetwork.isMasterClient && MyVariables.isrunningmod && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                string[] FileReadout = null;
                string[] ShipNames = null;
                try
                {
                    FileReadout = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt"));
                }
                catch
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FLEETMOD] UNABLE TO LOAD FLEETCONFIG");
                }
                if (FileReadout != null)
                {
                    foreach (string FileLines in FileReadout)
                    {
                        string[] FileSplit = null;
                        try { FileSplit = FileLines.Split('|'); }
                        catch { }
                        if (FileSplit != null && !Stored && FileSplit[0] == PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] == "FleetShip")
                        {
                            ShipNames.Append(FileSplit[2]);
                        }
                    }
                }
                return ShipNames;
            }
            else
            {
                return null;
            }
        }
    }
}
