using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using PulsarModLoader;
using UnityEngine;
using static PulsarModLoader.Patches.HarmonyHelpers;
namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLPlayer), "AttemptToTransferNeutralCargo")]
    internal class AttemptToTransferNeutralCargo
    {
        public static bool Prefix(int inCurrentShipID, int inNetID, PLPlayer __instance)
        {
            if (!MyVariables.isrunningmod)
            {
                return true;
            }
            else
            {
                if (PLEncounterManager.Instance != null)
                {
                    PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(inCurrentShipID) as PLShipInfo; // ? Gets current ship player is on
                    if (plshipInfo != null && !(CargoMenu.inNetID.Contains(inNetID) && CargoMenu.inCurrentShipID[CargoMenu.inNetID.IndexOf(inCurrentShipID)] == inCurrentShipID))
                    {
                        if (!MyVariables.CargoMenu)
                        {
                            MyVariables.CargoMenu = true;
                            CargoMenu.inCurrentShipID.Add(inCurrentShipID);
                            CargoMenu.inNetID.Add(inNetID);
                            var TransferMenu = new UnityEngine.GameObject("CargoMenu_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                            TransferMenu.AddComponent<CargoMenu>(); // Also TODO: Rename local vars...
                            UnityEngine.GameObject.DontDestroyOnLoad(TransferMenu);
                        }
                        else
                        {
                            CargoMenu.inCurrentShipID.Add(inCurrentShipID);
                            CargoMenu.inNetID.Add(inNetID);
                        }
                    }
                }
                return false;
            }
        }
    }

/*
namespace FLEETMOD.ModMessages
{
    internal class CargoTransferMenu : ModMessage
    {
        public override void HandleRPC(object[] arguments, /*arguments[0] = inCurrentShipID arguments[1] = inNetID*//* PhotonMessageInfo sender)
        {
            if (sender.sender == PhotonNetwork.masterClient && PLEncounterManager.Instance != null)
            {
                if (PLEncounterManager.Instance != null && int.TryParse(arguments[0].ToString(), out int inCurrentShipID) && int.TryParse(arguments[1].ToString(), out int inNetID))
                {
                    PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(inCurrentShipID) as PLShipInfo; // ? Gets current ship player is on
                    if (plshipInfo != null)
                    {
                        if (!MyVariables.CargoMenu)
                        {
                            MyVariables.CargoMenu = true;
                            CargoMenu.inCurrentShipID = inCurrentShipID;
                            CargoMenu.inNetID = inNetID;
                            var TransferMenu = new UnityEngine.GameObject("CargoMenu_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                            TransferMenu.AddComponent<CargoMenu>(); // Also TODO: Rename local vars...
                            UnityEngine.GameObject.DontDestroyOnLoad(TransferMenu);
                        }
                    }
                }
            }
        }
    }
}

                                                                                                                    
                                                                                                                    namespace FLEETMOD
                                                                                                                    {

                                                                                                                        [HarmonyPatch(typeof(PLPlayer), "AttemptToTransferNeutralCargo")]
                                                                                                                        internal class AttemptToTransferNeutralCargo
                                                                                                                        {
                                                                                                                            public static bool Prefix(int inCurrentShipID, int inNetID, PhotonMessageInfo sender)
                                                                                                                            {
                                                                                                                                if (!MyVariables.isrunningmod)
                                                                                                                                {
                                                                                                                                    return true;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    PulsarModLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.CargoTransferMenu", sender.sender, new object[] { });
                                                                                                                                    return false;
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                        
                                                                                                                    [HarmonyPatch(typeof(PLPlayer), "AttemptToTransferNeutralCargo")]
                                                                                                                    public class CargoTransferPatch
                                                                                                                    { /// AttemptToTransferNeutralCargo /// RPC replace with null
                                                                                                                        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                                                                                                                        {
                                                                                                                            List<CodeInstruction> findSequence = new List<CodeInstruction>()
                                                                                                                            {
                                                                                                                                new CodeInstruction(OpCodes.Ldarg_0),
                                                                                                                                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Time), "get_time")),
                                                                                                                                new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(PLGameStatic), "LastAttempttoTransferNeutralCargoTime")),
                                                                                                                                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(PLInput), "Instance")),
                                                                                                                                new CodeInstruction(OpCodes.Ldc_I4_S, 0x1C),
                                                                                                                                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLInput), "ResetHeldDownTime", new Type[] {typeof(PLInputBase) })),
                                                                                                                            };
                                                                                                                            int LabelIndex = FindSequence(instructions, findSequence, CheckMode.NEVER);
                                                                                                                            int LabelIndex2 = LabelIndex + 1;
                                                                                                                            PulsarModLoader.Utilities.Logger.Info($"[FM] {LabelIndex} | {instructions.ToList().Count()} | {instructions.ToList()[LabelIndex].opcode}");

                                                                                                                            List<CodeInstruction> targetSequence = new List<CodeInstruction>()
                                                                                                                            {
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2].opcode, instructions.ToList()[LabelIndex2].operand),
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 1].opcode, instructions.ToList()[LabelIndex2 + 1].operand),
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 2].opcode, instructions.ToList()[LabelIndex2 + 2].operand),
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 3].opcode, instructions.ToList()[LabelIndex2 + 3].operand),
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 4].opcode, instructions.ToList()[LabelIndex2 + 4].operand),
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 5].opcode, instructions.ToList()[LabelIndex2 + 5].operand),
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 6].opcode, instructions.ToList()[LabelIndex2 + 6].operand),
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 7].opcode, instructions.ToList()[LabelIndex2 + 7].operand),
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 8].opcode, instructions.ToList()[LabelIndex2 + 8].operand),
                                                                                                                                // PLNetworkManager.Instance.LocalPlayer.photonview.RPC("AttemptToTransferNeutralCargo", PhotonTargets.MasterClient, new object[]
                                                                                                                            };
                                                                                                                            List<CodeInstruction> injectedSequence = new List<CodeInstruction>()
                                                                                                                            {
                                                                                                                                // Replace with Null
                                                                                                                            };
                                                                                                                            return PatchBySequence(instructions, targetSequence, injectedSequence, patchMode: PatchMode.REPLACE, checkMode: CheckMode.NONNULL, showDebugOutput: true);
                                                                                                                        }
                                                                                                                    }
                                                                                                                    [HarmonyPatch(typeof(PLPlayer), "AttemptToTransferNeutralCargo")]
                                                                                                                    public class CargoTransferPatch2
                                                                                                                    { /// AttemptToTransferNeutralCargo /// Replace RPC call with custom method
                                                                                                                        static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
                                                                                                                        {
                                                                                                                            List<CodeInstruction> findSequence = new List<CodeInstruction>()
                                                                                                                            {
                                                                                                                                new CodeInstruction(OpCodes.Ldarg_0),
                                                                                                                                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Time), "get_time")),
                                                                                                                                new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(PLGameStatic), "LastAttempttoTransferNeutralCargoTime")),
                                                                                                                                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(PLInput), "Instance")),
                                                                                                                                new CodeInstruction(OpCodes.Ldc_I4_S, 0x1C),
                                                                                                                                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLInput), "ResetHeldDownTime", new Type[] {typeof(PLInputBase) })),
                                                                                                                            };
                                                                                                                            int LabelIndex = FindSequence(instructions, findSequence, CheckMode.NEVER);
                                                                                                                            int LabelIndex2 = LabelIndex + 1;
                                                                                                                            PulsarModLoader.Utilities.Logger.Info($"[FM] {LabelIndex} | {instructions.ToList().Count()} | {instructions.ToList()[LabelIndex].opcode}");

                                                                                                                            List<CodeInstruction> targetSequence = new List<CodeInstruction>()
                                                                                                                            {
                                                                                                                                new CodeInstruction(instructions.ToList()[LabelIndex2 + 21].opcode, instructions.ToList()[LabelIndex2 + 21].operand),
                                                                                                                                // callvirt RPC(string,valuetype photonTargets, object[])
                                                                                                                                // callvirt RPC(AttemptToTransferNeutralCargo, MasterClient, Current Ship ID  |  Cargo Net ID
                                                                                                                            };
                                                                                                                            List<CodeInstruction> injectedSequence = new List<CodeInstruction>()
                                                                                                                            {
                                                                                                                                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CargoTransfer), "CargoTransferMenu"))
                                                                                                                                // Call CargoMenu() instead of the RPC
                                                                                                                            };
                                                                                                                            return PatchBySequence(instructions, targetSequence, injectedSequence, patchMode: PatchMode.REPLACE, checkMode: CheckMode.NONNULL, showDebugOutput: true);
                                                                                                                        }
                                                                                                                    }
                                                                                                                    public class CargoTransfer
                                                                                                                    {
                                                                                                                        public static void CargoTransferMenu(int inCurrentShipID, int inNetID)
                                                                                                                        {
                                                                                                                            if (PLEncounterManager.Instance != null)
                                                                                                                            {
                                                                                                                                    PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(inCurrentShipID) as PLShipInfo; // ? Gets current ship player is on
                                                                                                                                    if (plshipInfo != null)
                                                                                                                                    {
                                                                                                                                        if (!MyVariables.CargoMenu)
                                                                                                                                        {
                                                                                                                                            MyVariables.CargoMenu = true;
                                                                                                                                            CargoMenu.inCurrentShipID.Append(inCurrentShipID);
                                                                                                                                            CargoMenu.inNetID.Append(inNetID);
                                                                                                                                            var TransferMenu = new UnityEngine.GameObject("CargoMenu_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                                                                                                                                            TransferMenu.AddComponent<CargoMenu>(); // Also TODO: Rename local vars...
                                                                                                                                            UnityEngine.GameObject.DontDestroyOnLoad(TransferMenu);
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }*/
}