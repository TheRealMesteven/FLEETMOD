using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using static HarmonyLib.AccessTools;
using static PulsarModLoader.Patches.HarmonyHelpers;
using static FLEETMOD.Interface.Tab.FleetShipListView;

namespace FLEETMOD.Interface.Tab
{
    internal class ExpandedCargo
    {
        /// <summary>
        /// Find existing tab features and curate the new implementations for the first time
        /// </summary>
        internal static void Initialize()
        {
            return;
        }

        /// <summary>
        /// First execution
        /// </summary>
        internal static void OnAwake()
        {
            return;
        }

        /// <summary>
        /// Update the tab features
        /// </summary>
        internal static void Update(PLTabMenu __instance)
        {
            if (!Variables.isrunningmod) return;
            Transform transform = PLTabMenu.Instance.SHIP_Stats1.gameObject.transform;
            if (__instance.TabMenuActive && __instance.ExpandedComponentView && __instance.CurrentTabIndex == 1)
            {
                // Attribute Override
                PLShipInfoBase Ship = null;
                if (ShipID == -1)
                {
                    Ship = PLNetworkManager.Instance.LocalPlayer.StartingShip;
                }
                else
                {
                    Ship = PLEncounterManager.Instance.GetShipFromID(ShipID);
                }
                if (Ship != null) 
                {
                    __instance.SHIP_Stats2.text = $"Currently Viewing\n{Ship.ShipNameValue} • {(PLNetworkManager.Instance.LocalPlayer.StartingShip == Ship ? "Your Ship" : "A Fleet Ship")}\n{Ship.GetShipTypeName()}";
                    __instance.SHIP_Stats2.enabled = true;
                }

                // Fleet Ship List
                __instance.SHIP_Stats1.text = "Fleetmod Ships";
                __instance.SHIP_Stats3.text = "Stats";
                __instance.SHIP_Stats1.enabled = true;
                __instance.SHIP_Stats3.enabled = true;

                if (transform != null && CurrentParent != transform)
                {
                    ChangeVisual(transform, new Vector3(0, 60, 0));
                    ShowShipList = true;
                }
            }
            else
            {
                if (transform != null && CurrentParent == transform)
                {
                    ChangeVisual(null, new Vector3(0, 0, 0));
                    ShowShipList = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLTabMenu), "UpdateSCDs")]
    class DisableCargoPageStats
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> target = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLLevelSync), "PlayerShip")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLShipInfoBase), "MyStats")),
                new CodeInstruction(OpCodes.Brfalse)
            }; //PLEncounterManager.Instance.PlayerShip.MyStats != null

            List<CodeInstruction> patch = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Call, Method(typeof(DisableCargoPageStats), "Replacement"))
            };

            int index = FindSequence(instructions, target, CheckMode.NONNULL);
            patch.Add(instructions.ToList()[index - 1]); // Used to differentiate between if statement and the for loop MyStats

            return PatchBySequence(instructions, target, new List<CodeInstruction>(), PatchMode.REPLACE, CheckMode.NONNULL);
        }
        public static bool Replacement()
        {
            return !Variables.isrunningmod && PLEncounterManager.Instance.PlayerShip.MyStats != null;
        }
    }
}
