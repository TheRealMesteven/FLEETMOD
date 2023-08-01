using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.SaveData;
using PulsarModLoader.Utilities;
using UnityEngine;

namespace FLEETMOD
{
    public class Mod : PulsarMod
    {
        public override string Version => Mod.myversion;
        public override string Author => "Mest, Dragon, Mikey, Badryuiner, Rayman";
        public override string Name => "FleetMod";
        public override string HarmonyIdentifier() => harmonyIden;
        public static string harmonyIden = "Mest.Fleetmod";
        public static string myversion = "FLEETMOD v1.8";

        class MySaveData : PMLSaveData
        {
            public static Dictionary<string, string> Ships = null;
            public override string Identifier() => "Mest.FleetMod";

            public override void LoadData(byte[] Data, uint VersionID)
            {
                Ships = DeSerializeFleetShips(Data);
            }
            public override byte[] SaveData() => SerializeFleetShips(GetFleetShips());
        }

        public static List<PLShipInfo> GetFleetShips()
        {
            List<PLShipInfo> Ships = new List<PLShipInfo>();
            foreach (PLShipInfoBase shipInfo in PLEncounterManager.Instance.AllShips.Values)
            { /// Code for saving each ship
				if (shipInfo != null && !shipInfo.GetHasBeenDestroyed() && shipInfo.GetIsPlayerShip() && !shipInfo.IsDrone)
                    {
                    Ships.Add((PLShipInfo)shipInfo);
                }
            }
            return Ships;
        }

        public static byte[] SerializeFleetShips(List<PLShipInfo> shipInfos)
        {
            MemoryStream dataStream = new MemoryStream();
            dataStream.Position = 0;
            using (BinaryWriter writer = new BinaryWriter(dataStream))
            {
                if (shipInfos.Count > 0)
                {
                    writer.Write(shipInfos.Count());
                    foreach (PLShipInfo shipInfo in shipInfos)
                    {
                        if (shipInfo == PLEncounterManager.Instance.PlayerShip)
                        {
                            writer.Write("");
                        }
                        else
                        {
                            writer.Write(shipInfo.ShipName);
                        }
                        string DataString = shipInfo.MyStats.CreateDataString();
                        PulsarModLoader.Utilities.Logger.Info($"[FLEETMOD] - Save {DataString}");
                        writer.Write(DataString);
                        //PulsarModLoader.Utilities.Messaging.Echo(PhotonNetwork.masterClient, $"[FLEETMOD] Saved {shipInfo.ShipName}");
                    }
                }
            }
            return dataStream.ToArray();
        }

        public static Dictionary<string, string> DeSerializeFleetShips(byte[] byteData)
        {
            MemoryStream memoryStream = new MemoryStream(byteData);
            memoryStream.Position = 0;
            try
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    int shipInfosCount = reader.ReadInt32();
                    Dictionary<string, string> shipInfos = new Dictionary<string, string>();
                    for (int i = 0; i < shipInfosCount; i++)
                    {
                        string Shipname = reader.ReadString();
                        string CrewString = reader.ReadString();
                        shipInfos.Add(Shipname, CrewString);
                    }
                    return shipInfos;
                }
            }
            catch (Exception ex)
            {
                PulsarModLoader.Utilities.Logger.Info($"Failed to read FleetShip List, returning null.\n{ex.Message}");
                return null;
            }
        }
        public static IEnumerator SpawnFleetShips()
        {/// ## Applying the saved values
            while (PLEncounterManager.Instance == null || PLEncounterManager.Instance.PlayerShip == null || MySaveData.Ships == null) yield return new WaitForEndOfFrame();
            foreach (KeyValuePair<string, string> keyValuePair in MySaveData.Ships)
            {
                SpawnShip(keyValuePair.Key, keyValuePair.Value);
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
        private static void SpawnShip(string Shipname, string CrewString)
        {
            if (Shipname == "") return;
            PLEncounterManager.ShipLayout shipLayout = new PLEncounterManager.ShipLayout(CrewString + ", " + PLEncounterManager.Instance.PlayerShip.MyStats.CreateCrewString());
            GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLPersistantEncounterInstance.GetPrefabNameForShipType(shipLayout.ShipType), new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
            gameObject.GetComponent<PLShipInfo>().SetShipID(PLServer.ServerSpaceTargetIDCounter++);
            gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
            gameObject.GetComponent<PLShipInfo>().TagID = -23;
            gameObject.GetComponent<PLShipInfo>().TeamID = 1;
            gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
            gameObject.GetComponent<PLShipInfo>().ShipNameValue = Shipname;
            gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
            gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);
            gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
            Variables.Fleet.Add(gameObject.GetComponent<PLShipInfo>().ShipID, new List<int>());
            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
            {
                "The " + Shipname + " Has Joined!",
                Color.green,
                0,
                "SHIP"
            });
            Variables.ReCalculateMaxPlayers();
        }
    }
    [HarmonyPatch(typeof(PLServer), "Start")]
    internal class FleetShipSpawning
    {
        public static void Postfix()
        {
            if (Variables.isrunningmod && PhotonNetwork.isMasterClient)
            {
                PLServer.Instance.StartCoroutine(Mod.SpawnFleetShips());
            }
        }
    }
    /*
    [HarmonyPatch(typeof(PLServer), nameof(PLServer.SpawnPlayerShipFromSaveData))]
    internal class Patch
    { /// ## Implementing applying of values
        static void Postfix()
        {
            if (PhotonNetwork.isMasterClient)
            {
                PLServer.Instance.StartCoroutine(Plugin.plugin.SpawnFleetShips());
            }
        }
    }
        /*
        public Plugin()  *** PATCHING FOR FUTURE SAVEGAME SHIP STORAGE ***
        {
            if (MyVariables.isrunningmod && PhotonNetwork.isMasterClient && PulsarModLoader.ModManager.Instance.IsModLoaded("CustomSave"))
            {
                Plugin.pos = new Dictionary<int, ShipPos>();
                ///<summary>
                /// Below lines read all of the saved ships data.
                /// <param name="key"> Crew ID of ship save</param>
                /// <param name="value"> Pos = Position of ship, will also save Name & Ship Layout </param>
                ///</summary>
                SaveManager.instance.RegisterReader(this, delegate (BinaryReader reader)
                {
                    Plugin.pos.Clear();
                    int num = reader.ReadInt32();
                    for (int i = 0; i < num; i++)
                    {
                        int key = reader.ReadInt32();
                        ShipPos value = default(ShipPos);
                        value.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        //value.hubid = reader.ReadInt32();
                        //value.ttiid = reader.ReadInt32();
                        Plugin.pos.Add(key, value);
                    }
                });
                ///<summary>
                /// Below lines write all of the saved ships data.
                /// <param name="key"> Crew ID of ship save</param>
                /// <param name="value"> Pos = Position of ship, will also save Name & Ship Layout </param>
                ///</summary>
                SaveManager.instance.RegisterWriter(this, delegate (BinaryWriter writer)
                {
                    Plugin.pos.Clear();
                    int Count = 0;
                    foreach (PLShipInfo plship in PLEncounterManager.Instance.AllShips.Values)
                    {
                        if (plship != null && plship.TagID == -23 && MyVariables.GetShipCaptain(plship.ShipID) != 0)
                        {
                            ShipPos value = new ShipPos
                            {
                                pos = plship.transform.position,
                                //hubid = plplayer.MyCurrentTLI.SubHubID,
                                //ttiid = plplayer.TTIID
                            };
                            Plugin.pos.Add(Count, value);
                            Count++;
                        }
                    }
                    writer.Write(Plugin.pos.Count);
                    foreach (KeyValuePair<int, ShipPos> keyValuePair in Plugin.pos)
                    {
                        writer.Write(keyValuePair.Key);
                        writer.Write(keyValuePair.Value.pos.x);
                        writer.Write(keyValuePair.Value.pos.y);
                        writer.Write(keyValuePair.Value.pos.z);
                        //writer.Write(keyValuePair.Value.hubid);
                        //writer.Write(keyValuePair.Value.ttiid);
                    }
                });
                ///
            }
        }
        internal static Dictionary<int, ShipPos> pos;
        internal struct ShipPos
        {
            public Vector3 pos;
        }*/

}
