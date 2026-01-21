using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine.UI;
using static HarmonyLib.AccessTools;
using static PulsarModLoader.Patches.HarmonyHelpers;

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
            if (__instance.TabMenuActive && __instance.ExpandedComponentView && __instance.CurrentTabIndex == 1)
            {
                __instance.SHIP_Stats1.text = "Fleetmod";
                __instance.SHIP_Stats2.text = "Says";
                __instance.SHIP_Stats3.text = "Hi";
                __instance.SHIP_Stats1.enabled = true;
                __instance.SHIP_Stats2.enabled = true;
                __instance.SHIP_Stats3.enabled = true;
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

            return PatchBySequence(instructions, target, new List<CodeInstruction>(), PatchMode.REPLACE, CheckMode.ALWAYS, showDebugOutput: false);
        }
        public static bool Replacement()
        {
            return !Variables.isrunningmod && PLEncounterManager.Instance.PlayerShip.MyStats != null;
        }
    }
}
