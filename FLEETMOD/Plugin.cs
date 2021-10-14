using System;
using System.Collections.Generic;
using System.IO;
using CustomSaves;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD
{
    public class Plugin : PulsarMod
    {
        public override string Version => Plugin.myversion;
        public override string Author => "Dragon+Mest";
        public override string Name => "FleetMod";
        public override int MPFunctionality => 0;
        public override string HarmonyIdentifier() => "Dragon+Mest.Fleetmod";
        public static string myversion = "FLEETMOD v1.51";
        /*
        public Plugin()
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
}
