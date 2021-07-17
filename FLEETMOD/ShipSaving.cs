using HarmonyLib;
using PulsarPluginLoader.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLSaveGameIO), "SaveToFile")]
    internal class FleetShipSaving
    {
        public static bool Prefix()
        {
            /// File Intended Layout
            /// Seed | FleetShip | Name | Ship Data
            if (PhotonNetwork.isMasterClient && MyVariables.isrunningmod && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                string[] FileReadout = null;
                StringBuilder stringBuilder = new StringBuilder();
                ///<summary>
                /// Below lines Reads / Generates the "FleetConfig.txt" file
                ///</summary>
                try
                {
                    FileReadout = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt"));
                }
                catch
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FLEETMOD] UNABLE TO LOAD FLEETCONFIG");
                }
                ///
                ///<summary>
                /// Below lines save only the non-fleet ships of the current galaxy information
                /// (Removes all of the saved fleet ships of the current galaxy)
                ///</summary>
                if (FileReadout != null)
                {
                    foreach (string FileLines in FileReadout)
                    {
                        string[] FileSplit = null;
                        try { FileSplit = FileLines.Split('|'); }
                        catch { }
                        if (FileSplit != null && FileSplit[0] != PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] != "FleetShip")
                        {
                            stringBuilder.AppendLine(FileLines);
                        }
                    }
                }
                ///
                ///<summary>
                /// Below lines append the fleet ships of this galaxy.
                ///</summary>
                foreach (PLShipInfo pLShipInfo in PLEncounterManager.Instance.AllShips.Values)
                {
                    if (pLShipInfo != null && pLShipInfo.TagID == -23 && PLServer.Instance.GetCachedFriendlyPlayerOfClass(0,pLShipInfo) != PLServer.Instance.GetPlayerFromPlayerID(0))
                    {
                        stringBuilder.AppendLine(PLServer.Instance.GalaxySeed.ToString() + "|" + "FleetShip" + "|" + pLShipInfo.ShipNameValue + "|" + pLShipInfo.MyStats.CreateDataString());
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt")))
                {
                    streamWriter.WriteLine(stringBuilder.ToString());
                }
                ///
            }
            return true;
        }
    }
    internal class ShipStorage
    {
        public static void StoreShip(int ShipID)
        {
            if (PhotonNetwork.isMasterClient && MyVariables.isrunningmod && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                string[] FileReadout = null;
                StringBuilder stringBuilder = new StringBuilder();
                ///<summary>
                /// Below lines Reads / Generates the "FleetConfig.txt" file
                ///</summary>
                try
                {
                    FileReadout = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt"));
                }
                catch
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FLEETMOD] UNABLE TO LOAD FLEETCONFIG");
                }
                ///
                ///<summary>
                /// Below lines save only the non-fleet ships of the current galaxy information
                /// (Removes all of the saved fleet ships of the current galaxy)
                ///</summary>
                if (FileReadout != null)
                {
                    foreach (string FileLines in FileReadout)
                    {
                        string[] FileSplit = null;
                        try { FileSplit = FileLines.Split('|'); }
                        catch { }
                        if (FileSplit != null && FileSplit[0] != PLServer.Instance.GalaxySeed.ToString() && FileSplit[1] != "FleetShip")
                        {
                            stringBuilder.AppendLine(FileLines);
                        }
                    }
                }
                ///
                ///<summary>
                /// Below lines append the fleet ships of this galaxy.
                ///</summary>
                foreach (PLShipInfo pLShipInfo in PLEncounterManager.Instance.AllShips.Values)
                {
                    if (pLShipInfo != null && pLShipInfo.TagID == -23 && PLServer.Instance.GetCachedFriendlyPlayerOfClass(0, pLShipInfo) != PLServer.Instance.GetPlayerFromPlayerID(0))
                    {
                        if (pLShipInfo.ShipID == ShipID)
                        {
                            stringBuilder.AppendLine(PLServer.Instance.GalaxySeed.ToString() + "|" + "StoredShip" + "|" + pLShipInfo.ShipNameValue + "|" + pLShipInfo.MyStats.CreateDataString());
                        }
                        else
                        {
                            stringBuilder.AppendLine(PLServer.Instance.GalaxySeed.ToString() + "|" + "FleetShip" + "|" + pLShipInfo.ShipNameValue + "|" + pLShipInfo.MyStats.CreateDataString());
                        }
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FleetConfig.txt")))
                {
                    streamWriter.WriteLine(stringBuilder.ToString());
                }
                PLEncounterManager.Instance.GetShipFromID(ShipID).TagID = -1;
                PLEncounterManager.Instance.GetShipFromID(ShipID).Ship_WarpOutNow();
                ///
            }
        }
    }
}
