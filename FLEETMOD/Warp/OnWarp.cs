using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ExitGames.Client.Photon.LoadBalancing;
using HarmonyLib;
using PulsarModLoader.Patches;
using PulsarModLoader.Utilities;
using UnityEngine;
using static PulsarModLoader.Patches.HarmonyHelpers;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLShipInfoBase), "OnWarp")]
    internal class OnWarp
    {
        public static bool Prefix(PLShipInfoBase __instance)
        {
            if (!Variables.isrunningmod) return true;
            if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip == __instance as PLShipInfo)
            {
                Dictionary<int, PlayerPos> UnModdedPositions = new Dictionary<int, PlayerPos>(); // PlayerID, PlayerPos
                foreach (int plshipID in Variables.Fleet.Keys)
                {
                    PLShipInfoBase plshipInfoBase = PLEncounterManager.Instance.GetShipFromID(plshipID);
                    if (plshipInfoBase != null && !plshipInfoBase.InWarp && plshipInfoBase != __instance)
                    {
                        foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                        {
                            if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.GetPhotonPlayer().GetScore() == plshipInfoBase.ShipID)
                            {
                                // Only teleport player back to ship if not on PlayerShip
                                PLTeleportationLocationInstance CurrentTLI = PLNetworkManager.Instance.LocalPlayer.MyCurrentTLI;
                                if (CurrentTLI == null || CurrentTLI.MyShipInfo == null || !Variables.Fleet.ContainsKey(CurrentTLI.MyShipInfo.ShipID))
                                {
                                    plplayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                                    {
                                        plshipInfoBase.MyTLI.SubHubID,
                                        0
                                    });
                                }

                                // Store the locations of NonModded Players for Teleporting
                                if (Variables.UnModdedCrews.ContainsKey(plplayer.GetPlayerID()))
                                {
                                    PlayerPos Position = new PlayerPos
                                    {
                                        pos = plplayer.GetPawn().transform.position,
                                        hubid = plplayer.MyCurrentTLI.SubHubID,
                                        ttiid = plplayer.TTIID
                                    };
                                    UnModdedPositions.Add(plplayer.GetPlayerID(), Position);
                                }

                                // Update Health Bonus On Warp
                                if (Variables.Modded.Contains(plplayer.GetPlayerID()))
                                {
                                    Variables.survivalBonusDict[plplayer.GetPlayerID()] = Mathf.Clamp(Variables.survivalBonusDict[plplayer.GetPlayerID()] + 1, -5, 20);
                                }
                            }
                        }

                        /*
                        // Only teleport player back to ship if not on PlayerShip
                        PLTeleportationLocationInstance CurrentTLI = PLNetworkManager.Instance.LocalPlayer.MyCurrentTLI;
                        if (CurrentTLI == null || CurrentTLI.MyShipInfo == null || !MyVariables.Fleet.ContainsKey(CurrentTLI.MyShipInfo.ShipID))
                        {
                            PLNetworkManager.Instance.LocalPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                            {
                                PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
                                0
                            });
                        }*/

                        // Generic Ship-Warp Value Updating
                        plshipInfoBase.WarpTravelDist = 1f;
                        plshipInfoBase.WarpTargetID = PLEncounterManager.Instance.PlayerShip.WarpTargetID;
                        plshipInfoBase.WarpTravelPercent = 0f;
                        plshipInfoBase.InWarp = true;
                        plshipInfoBase.OnWarp(plshipInfoBase.WarpTargetID);
                        plshipInfoBase.LastBeginWarpServerTime = PLEncounterManager.Instance.PlayerShip.LastBeginWarpServerTime;
                        plshipInfoBase.WarpChargeStage = EWarpChargeStage.E_WCS_ACTIVE;
                        PLWarpDrive shipComponent = plshipInfoBase.MyStats.GetShipComponent<PLWarpDrive>(ESlotType.E_COMP_WARP, false);
                        if (shipComponent != null)
                        {
                            shipComponent.OnWarpTo(PLEncounterManager.Instance.PlayerShip.WarpTargetID);
                        }
                        plshipInfoBase.ClearSendQueue();
                        PLShipInfo plshipInfo = plshipInfoBase as PLShipInfo;
                        if (plshipInfo != null)
                        {
                            plshipInfo.BlindJumpUnlocked = false;
                        }
                        plshipInfoBase.NumberOfFuelCapsules--;
                        plshipInfoBase.NumberOfFuelCapsules = Mathf.Clamp(plshipInfoBase.NumberOfFuelCapsules, 0, 200);
                        plshipInfoBase.AlertLevel = 0;
                    }
                }
                Variables.DialogGenerated = false;
                Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, "[WARP] - Update Mod Message");
                ModMessages.ServerUpdateVariables.UpdateClients();

                // Teleport unmodded players back to their positions when warping
                foreach (KeyValuePair<int, PlayerPos> keyValuePair in UnModdedPositions)
                {
                    PLPlayer player = PLServer.Instance.GetPlayerFromPlayerID(keyValuePair.Key);
                    player.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                    {
                                keyValuePair.Value.hubid,
                                keyValuePair.Value.ttiid
                    });
                    player.photonView.RPC("RecallPawnToPos", PhotonTargets.All, new object[]
                    {
                                keyValuePair.Value.pos
                    });
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PLShipControl), "FixedUpdate")]
    public class PatchWarpBundling
    { /// Patch ship stacking in warp
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*List<CodeInstruction> findSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Ldfld),
                new CodeInstruction(OpCodes.Ldc_R4),
                new CodeInstruction(OpCodes.Bge_Un_S),
                new CodeInstruction(OpCodes.Ldarg_0),
            };
            int LabelIndex = FindSequence(instructions, findSequence, CheckMode.NEVER);
            //PulsarModLoader.Utilities.Logger.Info($"[PM] Tag1  {LabelIndex} | {instructions.ToList().Count()}");
            //PulsarModLoader.Utilities.Logger.Info($"[PM] {instructions.ToList()[LabelIndex - 6].opcode}\n{instructions.ToList()[LabelIndex - 5].opcode}\n{instructions.ToList()[LabelIndex - 4].opcode}\n{instructions.ToList()[LabelIndex - 3].opcode}\n{instructions.ToList()[LabelIndex - 2].opcode}\n{instructions.ToList()[LabelIndex - 1].opcode}\n{instructions.ToList()[LabelIndex].opcode}");
            */
            List<CodeInstruction> targetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfo),"WarpObjectAlpha")),
                new CodeInstruction(OpCodes.Ldc_R4, 0.999f),

            };
            List<CodeInstruction> injectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfo),"WarpObjectAlpha")),
                new CodeInstruction(OpCodes.Ldc_R4, 1.01f),
            };
            return PatchBySequence(instructions, targetSequence, injectedSequence, patchMode: PatchMode.REPLACE, checkMode: CheckMode.NONNULL, showDebugOutput: true);
        }
    }
}
