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
                __instance.SHIP_Stats3.enabled = false;

                // Fleet Ship List Enable
                if (transform != null && CurrentParent != transform)
                {
                    ChangeVisual(transform, new Vector3(0, 60, 0));
                    ShowShipList = true;
                }
            }
            else
            {
                // Fleet Ship List Disable
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

    [HarmonyPatch(typeof(PLTabMenu), "UpdateSCDs")]
    class OverrideShipSlots
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Adds other ship cargos to the expanded cargo menu.
            CodeInstruction[] target = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLLevelSync), "PlayerShip")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLShipInfoBase), "MyStats")),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PLInventory), "GetAllSlots")),
            };

            CodeInstruction[] patch = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Call, Method(typeof(OverrideShipSlots), "GetShipSlots"))
            }; // Target:  PLEncounterManager.Instance.PlayerShip.MyStats.GetAllSlots()

            // More slots for Expanded Cargo

            CodeInstruction[] target2 = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldc_R4),
                new CodeInstruction(OpCodes.Stloc_S),
                new CodeInstruction(OpCodes.Ldc_R4),
                new CodeInstruction(OpCodes.Stloc_S),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S),
            };
            /*  Target:
                float num7 = 0f;
				float num8 = -30f;
				int num9 = 0;
				int num10 = 0;
				ESlotType eslotType = ESlotType.E_COMP_NONE;
				bool flag = false;
            */

            int index = FindSequence(instructions, target2, CheckMode.NONNULL);
            int index2 = FindSequence(instructions, target, CheckMode.NONNULL);

            List<CodeInstruction> patch2 = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, instructions.ToList()[index-15].operand),          // Transform transform2
                new CodeInstruction(OpCodes.Call, Method(typeof(OverrideShipSlots), "Replacement")),
                new CodeInstruction(instructions.ToList()[index2 - 8])                                  // int num4 = 9 or 12;
            };
            instructions = PatchBySequence(instructions, target2, patch2, PatchMode.AFTER, CheckMode.NONNULL);
            return PatchBySequence(instructions, target, patch, PatchMode.REPLACE, CheckMode.NONNULL); // Do replacement after the complicated one.
        }
        public static IEnumerable<PLSlot> GetShipSlots()
        {
            if (!Variables.isrunningmod) return PLEncounterManager.Instance.PlayerShip.MyStats.GetAllSlots().ToList();
            List<PLSlot> pLSlotItems = new List<PLSlot>();
            PLShipInfoBase pLShipInfoBase = PLEncounterManager.Instance.GetShipFromID(ShipID);
            if (pLShipInfoBase != null) pLSlotItems = pLShipInfoBase.MyStats.GetAllSlots().ToList();
            else if (PLNetworkManager.Instance.LocalPlayer.StartingShip != null) pLSlotItems = PLNetworkManager.Instance.LocalPlayer.StartingShip.MyStats.GetAllSlots().ToList();
            else if (PLEncounterManager.Instance.PlayerShip != null) pLSlotItems = PLEncounterManager.Instance.PlayerShip.MyStats.GetAllSlots().ToList();
            if (!PLTabMenu.Instance.ExpandedComponentView) return (IEnumerable<PLSlot>)pLSlotItems;
            foreach (int i in Variables.Fleet.Keys)
            {
                PLShipInfoBase pLShipInfoBase1 = PLEncounterManager.Instance.GetShipFromID(i);
                if (pLShipInfoBase1 != null)
                {
                    PLSlot Cargo = pLShipInfoBase1.MyStats.GetSlot(ESlotType.E_COMP_CARGO);
                    Cargo.Type = ESlotType.E_COMP_CARGO + (i*1000);
                    if (Cargo != null) pLSlotItems.Add(Cargo);
                    PLSlot HiddenCargo = pLShipInfoBase1.MyStats.GetSlot(ESlotType.E_COMP_HIDDENCARGO);
                    HiddenCargo.Type = ESlotType.E_COMP_HIDDENCARGO + (i*1000);
                    if (HiddenCargo != null) pLSlotItems.Add(HiddenCargo);
                }
            }
            return (IEnumerable<PLSlot>)pLSlotItems;
        }
        public static int Replacement(Transform transform2)
        {
            if (Variables.isrunningmod && PLTabMenu.Instance.ExpandedComponentView && transform2.name == "Components_Cargo")
            {
                return 12;
            }
            return 9;
        }
    }

    [HarmonyPatch(typeof(PLTabMenu), "UpdateSCDs")]
    class ExpandedExpandedCargo
    { // Other ship cargos go off screen if all rolands. So need to make more space.
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction[] target = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_R4),
                new CodeInstruction(OpCodes.Ldc_R4),
                new CodeInstruction(OpCodes.Ldc_R4),
                new CodeInstruction(OpCodes.Newobj),
                new CodeInstruction(OpCodes.Stfld)
            };//this.ShipComponentsGrid_Basics_TargetLocalPosition = new Vector3(-800f, 600f, 0f);

            int index = FindSequence(instructions, target, CheckMode.NONNULL);
            List<CodeInstruction> PatchedInstructions = instructions.ToList();
            PatchedInstructions[index - 5].operand = -1000f; // ShipComponentsGrid_Basics_TargetLocalPosition.x
            PatchedInstructions[index - 4].operand = 700f; // ShipComponentsGrid_Basics_TargetLocalPosition.y
            PatchedInstructions[index + 1].operand = -200f;  // ShipComponentsGrid_Computer_TargetLocalPosition.x
            PatchedInstructions[index + 2].operand = 700f;  // ShipComponentsGrid_Computer_TargetLocalPosition.y
            PatchedInstructions[index + 7].operand = -1000f; // ShipComponentsGrid_Weapons_TargetLocalPosition.x
            PatchedInstructions[index + 8].operand = 30f; // ShipComponentsGrid_Weapons_TargetLocalPosition.y
            PatchedInstructions[index + 13].operand = 600f;  // ShipComponentsGrid_Cargo_TargetLocalPosition.x
            PatchedInstructions[index + 14].operand = 700f;  // ShipComponentsGrid_Cargo_TargetLocalPosition.y
            PatchedInstructions[index + 19].operand = -200f; // ShipComponentsGrid_Thrusters_TargetLocalPosition.x
            PatchedInstructions[index + 20].operand = 30f; // ShipComponentsGrid_Thrusters_TargetLocalPosition.y

            return (IEnumerable<CodeInstruction>)PatchedInstructions;
        }
    }

    [HarmonyPatch(typeof(PLTabMenu), "GetComponentParent")]
    class AddCargoGrids
    {
        public static bool Prefix(ESlotType inSlotType, ref int transformIndex, ref Transform __result)
        {
            if ((int)inSlotType > 1000)
            {
                transformIndex = 3;
                __result = PLTabMenu.Instance.ShipComponentsGrid_Cargo;
                return false;
            }
            /*if (inSlotType == ESlotType.E_COMP_AIRLOCK)
            {
                transformIndex = 3;
                __result = PLTabMenu.Instance.ShipComponentsGrid_Cargo;
                return false;
            }*/
            return true;
        }
    }

    [HarmonyPatch(typeof(PLShipComponent), "GetStringForType")]
    class RenameCargoGrids
    {
        public static bool Prefix(ESlotType inSlotType, ref string __result)
        {
            if ((int)inSlotType > 1000)
            {
                int ShipID = (int)inSlotType / 1000;
                int SlotType = (int)inSlotType % 1000;
                PLShipInfoBase ship = PLEncounterManager.Instance.GetShipFromID(ShipID);
                if (ship != null)
                {
                    if (SlotType == (int)ESlotType.E_COMP_CARGO) __result = $"{ship.ShipNameValue}'s Cargo";
                    else __result = $"{ship.ShipNameValue}'s Hidden Cargo";
                }
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(PLDraggedShipCompUI), "Update")]
    class OverrideShipForRearrange
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction[] target = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLLevelSync), "PlayerShip")),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PLShipInfoBase), "get_ShipID")),
            };
            /* 
             * Target:  PLEncounterManager.Instance.PlayerShip.ShipID
            */
            return PatchBySequence(instructions,
            target, new CodeInstruction[] {
                new CodeInstruction(OpCodes.Call, Method(typeof(OverrideShipForRearrange), "GetShipForRearrange"))
            }, PatchMode.REPLACE, CheckMode.NONNULL);
        }
        public static int GetShipForRearrange()
        {
            if (!Variables.isrunningmod) return PLEncounterManager.Instance.PlayerShip.ShipID;
            PLShipInfoBase pLShipInfoBase = PLEncounterManager.Instance.GetShipFromID(ShipID);
            if (pLShipInfoBase != null) return ShipID;
            else if (PLNetworkManager.Instance.LocalPlayer.StartingShip != null) return PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID;
            return PLEncounterManager.Instance.PlayerShip.ShipID;
        }
    }
}