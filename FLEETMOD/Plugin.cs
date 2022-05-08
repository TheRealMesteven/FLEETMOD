using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CustomSaves;
using HarmonyLib;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD
{
    public class Plugin : PulsarMod
    {
        public override string Version => Plugin.myversion;
        public override string Author => "Mest, Dragon, Mikey, Badryuiner, Rayman";
        public override string Name => "FleetMod";
        public override int MPFunctionality => 0;
        public override string HarmonyIdentifier() => "Dragon+Mest.Fleetmod";
        public static string myversion = "FLEETMOD v1.6.5.1";

        public Plugin()
        { /// ## Setup where config default is estabilished and values read
            CustomSaves.SaveManager.Instance.RegisterReaderAndWriter("FleetmodShips", AuxReader, AuxWriter);
            config = new Plugin.FleetmodShips { Ship0 = null, Ship1 = null, Ship2 = null, Ship3 = null, Ship4 = null, Ship5 = null, Ship6 = null };
            plugin = this;
        }

        private void AuxReader(BinaryReader reader)
        { /// Maximum 7 additional ships saved
            config.Ship0 = reader.ReadString();  	// 0
            config.Ship1 = reader.ReadString();  	// 1
            config.Ship2 = reader.ReadString();     // 2
            config.Ship3 = reader.ReadString();  	// 3
            config.Ship4 = reader.ReadString();    	// 4
            config.Ship5 = reader.ReadString();   	// 5
            config.Ship6 = reader.ReadString();		// 6 
        }
        private void AuxWriter(BinaryWriter writer)
        {
            foreach (PLShipInfoBase shipInfo in PLEncounterManager.Instance.AllShips.Values)
            { /// Code for saving each ship
				if (shipInfo != null && shipInfo.GetIsPlayerShip() && shipInfo.ShipID != PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore())
                {
                    writer.Write(shipInfo.ShipNameValue + "⍰" + shipInfo.MyStats.CreateDataString());
                }
            }
        }

        internal static FleetmodShips config;
        internal static Plugin plugin;

        internal struct FleetmodShips
        {
            public string Ship0, Ship1, Ship2, Ship3, Ship4, Ship5, Ship6;
        }

        internal IEnumerator SpawnFleetShips()
        {/// ## Applying the saved values
            while ((PLEncounterManager.Instance == null) || (PLEncounterManager.Instance.PlayerShip == null)) yield return new WaitForEndOfFrame();
            if (Plugin.config.Ship0 != null)
            {
                SpawnShip(Plugin.config.Ship0);
            }
            yield return new WaitForEndOfFrame();
            if (Plugin.config.Ship1 != null)
            {
                SpawnShip(Plugin.config.Ship1);
            }
            yield return new WaitForEndOfFrame();
            if (Plugin.config.Ship2 != null)
            {
                SpawnShip(Plugin.config.Ship2);
            }
            yield return new WaitForEndOfFrame();
            if (Plugin.config.Ship3 != null)
            {
                SpawnShip(Plugin.config.Ship3);
            }
            yield return new WaitForEndOfFrame();
            if (Plugin.config.Ship4 != null)
            {
                SpawnShip(Plugin.config.Ship4);
            }
            yield return new WaitForEndOfFrame();
            if (Plugin.config.Ship5 != null)
            {
                SpawnShip(Plugin.config.Ship5);
            }
            yield return new WaitForEndOfFrame();
        }
        private void SpawnShip(string ShipCode)
        {
            PLEncounterManager.ShipLayout shipLayout = new PLEncounterManager.ShipLayout(ShipCode.Split('⍰')[1] + ", " + PLEncounterManager.Instance.PlayerShip.MyStats.CreateCrewString());
            GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLPersistantEncounterInstance.GetPrefabNameForShipType(shipLayout.ShipType), new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
            gameObject.GetComponent<PLShipInfo>().SetShipID(PLServer.ServerSpaceTargetIDCounter++);
            gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
            gameObject.GetComponent<PLShipInfo>().TagID = -23;
            gameObject.GetComponent<PLShipInfo>().TeamID = 1;
            gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
            gameObject.GetComponent<PLShipInfo>().ShipNameValue = ShipCode.Split('⍰')[0];
            gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
            gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);
            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
            {
                "The " + ShipCode.Split('⍰')[0] + " Has Joined!",
                Color.green,
                0,
                "SHIP"
            });
        }
    }
    [HarmonyPatch(typeof(PLServer), "ServerCaptainStartGame")]
    internal class FleetShipSpawning
    {
        public static void Postfix()
        {
            if (MyVariables.isrunningmod && PhotonNetwork.isMasterClient)
            {
                PLServer.Instance.StartCoroutine(Plugin.plugin.SpawnFleetShips());
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
