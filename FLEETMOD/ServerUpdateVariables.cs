using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD
{   
    internal class ServerUpdateVariables : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (!PhotonNetwork.isMasterClient && sender.sender == PhotonNetwork.masterClient)
            {
                DeSerializeSyncValues(arguments.Cast<byte>().ToArray());
            }
        }
        public static void UpdateClients()
        {
            if (PLEncounterManager.Instance.PlayerShip != null)
            {
                foreach (int PlayerID in MyVariables.Modded)
                {
                    PLPlayer player = PLServer.Instance.GetPlayerFromPlayerID(PlayerID);
                    if (player != null)
                    {
                        PulsarModLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ServerUpdateVariables", player.GetPhotonPlayer(), SerializeSyncValues().Cast<object>().ToArray());
                    }
                }
            }
        }
        public static byte[] SerializeSyncValues()
        {
            MemoryStream dataStream = new MemoryStream();
            dataStream.Position = 0;
            using (BinaryWriter writer = new BinaryWriter(dataStream))
            {
                writer.Write(MyVariables.shipfriendlyfire);
                writer.Write(MyVariables.recentfriendlyfire);
                writer.Write(MyVariables.survivalBonusDict.Count);
                foreach (KeyValuePair<int, int> survivalBonusPair in MyVariables.survivalBonusDict)
                {
                    writer.Write(survivalBonusPair.Key);
                    writer.Write(survivalBonusPair.Value);
                }
                writer.Write(MyVariables.Fleet.Count);
                foreach (KeyValuePair<int, List<int>> FleetPair in MyVariables.Fleet)
                {
                    writer.Write(FleetPair.Key);
                    writer.Write(FleetPair.Value.Count);
                    foreach (int FleetCrewMember in FleetPair.Value)
                    {
                        writer.Write(FleetCrewMember);
                    }
                }
                writer.Write(MyVariables.Modded.Count);
                foreach (int ModdedID in MyVariables.Modded)
                {
                    writer.Write(ModdedID);
                }
                writer.Write(MyVariables.NonModded.Count);
                foreach (int NonModdedID in MyVariables.NonModded)
                {
                    writer.Write(NonModdedID);
                }
            }
            return dataStream.ToArray();
        }
        public static void DeSerializeSyncValues(byte[] byteData)
        {
            MemoryStream memoryStream = new MemoryStream(byteData);
            memoryStream.Position = 0;
            try
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    MyVariables.shipfriendlyfire = reader.ReadBoolean();
                    MyVariables.recentfriendlyfire = reader.ReadBoolean();
                    Dictionary<int, int> survivalBonus = new Dictionary<int, int>();
                    int survivalBonusCount = reader.ReadInt32();
                    for (int i = 0; i < survivalBonusCount; i++)
                    {
                        int Key = reader.ReadInt32();
                        int Value = reader.ReadInt32();
                        survivalBonus.Add(Key, Value);
                    }
                    MyVariables.survivalBonusDict = survivalBonus;
                    Dictionary<int, List<int>> Fleet = new Dictionary<int, List<int>>();
                    int FleetCount = reader.ReadInt32();
                    for (int i = 0; i < FleetCount; i++)
                    {
                        int ShipID = reader.ReadInt32();
                        List<int> Crew = new List<int>();
                        int ValueCount = reader.ReadInt32();
                        for (int j = 0; j < ValueCount; j++)
                        {
                            Crew.Add(reader.ReadInt32());
                        }
                        Fleet.Add(ShipID, Crew);
                    }
                    MyVariables.Fleet = Fleet;
                    List<int> Modded = new List<int>();
                    int ModdedCount = reader.ReadInt32();
                    for (int i = 0; i < ModdedCount; i++)
                    {
                        Modded.Add(reader.ReadInt32());
                    }
                    MyVariables.Modded = Modded;
                    List<int> NonModded = new List<int>();
                    int NonModdedCount = reader.ReadInt32();
                    for (int i = 0; i < NonModdedCount; i++)
                    {
                        NonModded.Add(reader.ReadInt32());
                    }
                    MyVariables.NonModded = NonModded;
                }
            }
            catch (Exception ex)
            {
                PulsarModLoader.Utilities.Logger.Info($"Failed to read Serialized Values, returning null.\n{ex.Message}");
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "LoginMessage")] //Initial Sync. If multiple messages exist consider making new Modmessage for initial sync
    class LoginMessagePatch
    {
        static void Postfix(PhotonPlayer newPhotonPlayer)
        {
            if (PhotonNetwork.isMasterClient)
            {
                ModMessage.SendRPC(Plugin.harmonyIden, "FLEETMOD.ActivateFleetmod", newPhotonPlayer, new object[] { });
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "Start")]
    class StartPatch
    { /// Initial Patch creating the dictionaries and lists.
        static void Postfix()
        {
            MyVariables.ShipCrews = new Dictionary<PLShipInfo, int>();
            MyVariables.survivalBonusDict = new Dictionary<int, int>();
            MyVariables.Fleet = new Dictionary<int, List<int>>();
            MyVariables.DialogGenerated = false;
            MyVariables.Modded = new List<int>();
            MyVariables.NonModded = new List<int>();
            if (PhotonNetwork.isMasterClient)
            {
                MyVariables.UnModdedCrews = new Dictionary<int, int>();
                foreach (PulsarMod pulsarMod in ModManager.Instance.GetAllMods())
                {
                    if (pulsarMod.HarmonyIdentifier() == "mod.id107.beammeup")
                    {
                        PulsarModLoader.Utilities.Logger.Info("Fleetmod has disabled " + pulsarMod.Name + " due to mod conflicts.");
                        pulsarMod.Unload();
                    }
                }
            }   
        }
    }
}
