using PulsarModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD.ModMessages
{
    internal class ServerUpdateVariables : ModMessage
    {
        private static int Count = 0;
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (!PhotonNetwork.isMasterClient && sender.sender == PhotonNetwork.masterClient)
            {
                //PulsarModLoader.Utilities.Messaging.Echo(sender.sender, $"Update RPC {Count++} recieved");
                DeSerializeSyncValues(arguments.Cast<byte>().ToArray());
            }
        }
        public static void UpdateClients()
        {
            if (PLEncounterManager.Instance.PlayerShip != null)
            {
                foreach (int PlayerID in Variables.Modded)
                {
                    PLPlayer player = PLServer.Instance.GetPlayerFromPlayerID(PlayerID);
                    if (player != null)
                    {
                        ModMessage.SendRPC(Mod.harmonyIden, "FLEETMOD.ModMessages.ServerUpdateVariables", player.GetPhotonPlayer(), SerializeSyncValues().Cast<object>().ToArray());
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
                writer.Write(Variables.survivalBonusDict.Count);
                foreach (KeyValuePair<int, int> survivalBonusPair in Variables.survivalBonusDict)
                {
                    writer.Write(survivalBonusPair.Key);
                    writer.Write(survivalBonusPair.Value);
                }
                writer.Write(Variables.Fleet.Count);
                foreach (KeyValuePair<int, List<int>> FleetPair in Variables.Fleet)
                {
                    writer.Write(FleetPair.Key);
                    writer.Write(FleetPair.Value.Count);
                    foreach (int FleetCrewMember in FleetPair.Value)
                    {
                        writer.Write(FleetCrewMember);
                    }
                }
                writer.Write(Variables.Modded.Count);
                foreach (int ModdedID in Variables.Modded)
                {
                    writer.Write(ModdedID);
                }
                writer.Write(Variables.NonModded.Count);
                foreach (int NonModdedID in Variables.NonModded)
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
                    Dictionary<int, int> survivalBonus = new Dictionary<int, int>();
                    int survivalBonusCount = reader.ReadInt32();
                    for (int i = 0; i < survivalBonusCount; i++)
                    {
                        int Key = reader.ReadInt32();
                        int Value = reader.ReadInt32();
                        survivalBonus.Add(Key, Value);
                    }
                    Variables.survivalBonusDict = survivalBonus;
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
                    Variables.Fleet = Fleet;
                    List<int> Modded = new List<int>();
                    int ModdedCount = reader.ReadInt32();
                    for (int i = 0; i < ModdedCount; i++)
                    {
                        Modded.Add(reader.ReadInt32());
                    }
                    Variables.Modded = Modded;
                    List<int> NonModded = new List<int>();
                    int NonModdedCount = reader.ReadInt32();
                    for (int i = 0; i < NonModdedCount; i++)
                    {
                        NonModded.Add(reader.ReadInt32());
                    }
                    Variables.NonModded = NonModded;
                }
            }
            catch (Exception ex)
            {
                PulsarModLoader.Utilities.Logger.Info($"Failed to read Serialized Values, returning null.\n{ex.Message}");
            }
        }
    }
}
