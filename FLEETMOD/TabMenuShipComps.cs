using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static PulsarModLoader.Patches.HarmonyHelpers;
using static PulsarModLoader.Utilities.Logger;
using static HarmonyLib.AccessTools;
using PulsarModLoader.Utilities;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLTabMenu), "UpdateSCDs")]
    class TabMenuSlotOverride
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> target = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLLevelSync), "PlayerShip")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLShipInfoBase), "MyStats")),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PLInventory), "GetAllSlots"))
            };

            List<CodeInstruction> patch = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Call, Method(typeof(TabMenuSlotOverride), "pLSlotItems"))
            };

            return PatchBySequence(instructions, target, patch, PatchMode.REPLACE);
        }
        public static IEnumerable<PLSlot> pLSlotItems (PLTabMenu __instance)
        {
            Dictionary<ESlotType, PLSlot> keyValuePairs = new Dictionary<ESlotType, PLSlot>();
            int CargoSlotMaximum = 0;
            //int HiddenCargoSlotMaximum = 0;
            if (!MyVariables.isrunningmod || MyVariables.Fleet == null || MyVariables.Fleet.Count <= 1)
            {
                return PLEncounterManager.Instance.PlayerShip.MyStats.GetAllSlots();
            }
            foreach (int ShipID in MyVariables.Fleet.Keys)
            {
                PLShipInfo shipInfo = (PLShipInfo)PLEncounterManager.Instance.GetShipFromID(ShipID);
                if (shipInfo != null && shipInfo.MyStats != null && shipInfo.GetLifetime() > 2f && !shipInfo.IsDrone && !shipInfo.HasBeenDestroyed)
                {
                    if (shipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO) != null)
                    {
                        CargoSlotMaximum += shipInfo.CargoBases.Count();
                    }
                    if (shipInfo == PLEncounterManager.Instance.PlayerShip)
                    {
                        foreach (ESlotType slotType in Enum.GetValues(typeof(ESlotType)))
                        {
                            if (keyValuePairs.ContainsKey(slotType))
                            {
                                keyValuePairs[slotType].AddRange(shipInfo.MyStats.GetSlot(slotType));
                            }
                            else
                            {
                                keyValuePairs.Add(slotType, shipInfo.MyStats.GetSlot(slotType));
                            }
                        }
                    }
                    else
                    {
                        if (shipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO) != null)
                        {
                            if (keyValuePairs.ContainsKey(ESlotType.E_COMP_CARGO))
                            {
                                keyValuePairs[ESlotType.E_COMP_CARGO].AddRange(shipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO));
                            }
                            else
                            {
                                keyValuePairs.Add(ESlotType.E_COMP_CARGO, shipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO));
                            }
                        }
                        /*if (keyValuePairs.ContainsKey(ESlotType.E_COMP_HIDDENCARGO))
                        {
                            keyValuePairs[ESlotType.E_COMP_HIDDENCARGO].AddRange(shipInfo.MyStats.GetSlot(ESlotType.E_COMP_HIDDENCARGO));
                        }
                        else
                        {
                            keyValuePairs.Add(ESlotType.E_COMP_HIDDENCARGO, shipInfo.MyStats.GetSlot(ESlotType.E_COMP_HIDDENCARGO));
                        }*/
                    }

                }
            }
            keyValuePairs[ESlotType.E_COMP_CARGO].MaxItems = CargoSlotMaximum;
            return keyValuePairs.Values;
        }
    }
}
