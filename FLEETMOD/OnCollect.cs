using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using PulsarModLoader;
using UnityEngine;
using static HarmonyLib.AccessTools;
using static PulsarModLoader.Utilities.Logger;
using static PulsarModLoader.Patches.HarmonyHelpers;
using static System.Reflection.Emit.OpCodes;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLSpaceScrap), "OnCollect")]
    class SpaceSrapOnCollect
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> target = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLLevelSync), "PlayerShip"))
            }; // PLShipInfo plshipInfo = PLEncounterManager.Instance.PlayerShip;

            List<CodeInstruction> patch = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Call, Method(typeof(PlayerShipReplace), "PatchShip"))
            }; // result: PLShipInfo plshipInfo = FLEETMOD.PlayerShipReplace.PatchShip(this);

            return PatchBySequence(instructions, target, patch, PatchMode.REPLACE, showDebugOutput: false);
        }
    }

    [HarmonyPatch(typeof(PLSpaceScrap), "Update")]
    class SpaceScrapUpdate
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return PatchBySequence(instructions,
                new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLLevelSync), "PlayerShip")),
                new CodeInstruction(OpCodes.Stloc_0)
                },
                    new CodeInstruction[]
                {
                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Call, Method(typeof(PlayerShipReplace), "PatchShip")),
                new CodeInstruction(OpCodes.Stloc_0)
    }, PatchMode.AFTER, CheckMode.ALWAYS, false);
            /*
             * Before: PLShipInfo plshipInfo = PLEncounterManager.Instance.PlayerShip;
             * After: PLShipInfo plshipInfo = PLEncounterManager.Instance.PlayerShip;
             *       plshipInfo = FLEETMOD.PlayerShipReplace.PatchShip(this);
             */
        }
    }

    [HarmonyPatch(typeof(PLShipInfo), "UpdateSensorDish")]
    class UpdateSensorDishPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return PatchBySequence(instructions,
                new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, Method(typeof(Photon.MonoBehaviour), "get_photonView")),
                    new CodeInstruction(OpCodes.Ldstr, "RequestScrapCollectFromSensorDish"),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Newarr, typeof(System.Object)),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLShipInfo), "SensorDishCurrentSecondaryTarget_Scrap")),
                    new CodeInstruction(OpCodes.Callvirt, Method(typeof(PLSpecialEncounterNetObject), "get_EncounterNetID")),
                    new CodeInstruction(OpCodes.Box, typeof(int)),
                    new CodeInstruction(OpCodes.Stelem_Ref),
                    new CodeInstruction(OpCodes.Callvirt, Method(typeof(PhotonView), "RPC", new[] { typeof(string), typeof(PhotonTargets), typeof(object[]) }))
                    /*
                     base.photonView.RPC("RequestScrapCollectFromSensorDish", PhotonTargets.MasterClient, new object[]
				    {
					    this.SensorDishCurrentSecondaryTarget_Scrap.EncounterNetID
				    });
                     */
                    },
                    new CodeInstruction[]
                    {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLShipInfo), "SensorDishCurrentSecondaryTarget_Scrap")),
                    new CodeInstruction(OpCodes.Callvirt, Method(typeof(PLSpecialEncounterNetObject), "get_EncounterNetID")),
                    new CodeInstruction(OpCodes.Call, Method(typeof(UpdateSensorDishPatch), "Fix"))
                    }, PatchMode.REPLACE, CheckMode.ALWAYS, false);
        }

        public static void Fix(int NetID)
        {
            ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.SensorDishCollectScrap", PhotonTargets.MasterClient, new object[]
            {
                NetID,
                PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.ShipID
            });
        }
    }

    public class PlayerShipReplace
    {
        public static PLShipInfo PatchShip(PLSpaceScrap scrap) // Low on performance?
        {
            if (scrap != null)
            {
                List<PLShipInfo> allFleetShips = new List<PLShipInfo>();
                foreach (PLShipInfo fleetship in PLEncounterManager.Instance.AllShips.Values)
                {
                    if (fleetship != null && !fleetship.IsDrone)
                    {
                        allFleetShips.Add(fleetship);
                    }
                    //Info($"NewShip: {fleetship.Key} - {((PLShipInfo)PLEncounterManager.Instance.GetShipFromID(fleetship.Value)).ShipName}");
                }

                var pos = scrap.transform.position;
                float dist = float.MaxValue;
                PLShipInfo selectedShip = PLEncounterManager.Instance.PlayerShip; // anti-null

                foreach (var ship in allFleetShips)
                {
                    var localdist = Vector3.Distance(ship.GetSpaceLoc(), pos);
                    //Info($"{localdist.ToString()} vs {dist} - {ship.ShipName}");
                    if (dist > localdist)
                    {
                        dist = localdist;
                        selectedShip = ship;
                    }
                }
                return selectedShip;
            }

            return PLEncounterManager.Instance.PlayerShip;
        }
    }
}