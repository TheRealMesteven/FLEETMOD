using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using PulsarPluginLoader;
using UnityEngine;
using static HarmonyLib.AccessTools;
using static PulsarPluginLoader.Utilities.Logger;
using static PulsarPluginLoader.Patches.HarmonyHelpers;
using static System.Reflection.Emit.OpCodes;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLSpaceScrap), "OnCollect")]
    class SpaceSrapOnCollect
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> target = new()
            {
                new(Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new(Ldfld, Field(typeof(PLLevelSync), "PlayerShip"))
            }; // PLShipInfo plshipInfo = PLEncounterManager.Instance.PlayerShip;

            List<CodeInstruction> patch = new()
            {
                new(Ldarg_0), // this
                new(Call, Method(typeof(PlayerShipReplace), "PatchShip"))
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
                new(Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new(Ldfld, Field(typeof(PLLevelSync), "PlayerShip")),
                new(Stloc_0)
            }, 
                new CodeInstruction[]
            {
                new(Ldarg_0), // this
                new(Call, Method(typeof(PlayerShipReplace), "PatchShip")),
                new(Stloc_0) 
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
                    new(Ldarg_0),
                    new(Call, Method(typeof(Photon.MonoBehaviour), "get_photonView")),
                    new(Ldstr, "RequestScrapCollectFromSensorDish"),
                    new(Ldc_I4_2),
                    new(Ldc_I4_1),
                    new(Newarr, typeof(System.Object)),
                    new(Dup),
                    new(Ldc_I4_0),
                    new(Ldarg_0),
                    new(Ldfld,Field(typeof(PLShipInfo), "SensorDishCurrentSecondaryTarget_Scrap")),
                    new(Callvirt, Method(typeof(PLSpecialEncounterNetObject), "get_EncounterNetID")),
                    new(Box, typeof(int)),
                    new(Stelem_Ref),
                    new(Callvirt, Method(typeof(PhotonView), "RPC", new []{ typeof(string), typeof(PhotonTargets), typeof(object[])}))
                    /*
                     base.photonView.RPC("RequestScrapCollectFromSensorDish", PhotonTargets.MasterClient, new object[]
				    {
					    this.SensorDishCurrentSecondaryTarget_Scrap.EncounterNetID
				    });
                     */
                },
                new CodeInstruction[]
                {
                    new(Ldarg_0),
                    new(Ldfld,Field(typeof(PLShipInfo), "SensorDishCurrentSecondaryTarget_Scrap")),
                    new(Callvirt, Method(typeof(PLSpecialEncounterNetObject), "get_EncounterNetID")),
                    new(Call, Method(typeof(UpdateSensorDishPatch), "Fix"))
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
                List<PLShipInfo> allFleetShips = new();
                foreach (var fleetship in Global.Fleet)
                {
                    allFleetShips.Add((PLShipInfo)PLEncounterManager.Instance.GetShipFromID(fleetship.Value));
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
