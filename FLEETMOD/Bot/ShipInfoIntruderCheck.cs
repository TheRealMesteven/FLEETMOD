using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using static PulsarModLoader.Patches.HarmonyHelpers;
using static HarmonyLib.AccessTools;
using System.Linq;

namespace FLEETMOD.Bot
{
    [HarmonyPatch(typeof(PLShipInfo), "Update")]
    class ShipInfoIntruderCheck
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> targetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PLCombatTarget), "GetPlayer")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLPlayer), "TeamID")),
                new CodeInstruction(OpCodes.Call),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLShipInfoBase), "TeamID")),
                new CodeInstruction(OpCodes.Beq_S)
            }; // .GetPlayer().TeamID != this.TeamID
            int LabelIndex = FindSequence(instructions, targetSequence, CheckMode.NONNULL) - 1;
            List<CodeInstruction> patchSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Call, Method(typeof(ShipInfoIntruderCheck), "Replacement")),
                new CodeInstruction(OpCodes.Brfalse, instructions.ToList()[LabelIndex].operand)
            };

            return PatchBySequence(instructions, targetSequence, patchSequence, PatchMode.REPLACE, CheckMode.NONNULL);
        }
        static bool Replacement(PLPlayer pLPlayer, PLShipInfo pLShipInfo)
            => pLPlayer.StartingShip == pLShipInfo;
    }
}
