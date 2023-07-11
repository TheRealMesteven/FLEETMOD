using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using static PulsarModLoader.Patches.HarmonyHelpers;
using static HarmonyLib.AccessTools;
using System.Linq;

namespace FLEETMOD.Bot
{
    [HarmonyPatch]
    class BotPriorityPatches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PLAIPriorityOverride), "IsActive")]
        static IEnumerable<CodeInstruction> OnHostileShip(IEnumerable<CodeInstruction> instructions) // case: 20
        {
            List<CodeInstruction> targetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLPlayer), "MyCurrentTLI")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLTeleportationLocationInstance), "MyShipInfo")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLShipInfoBase), "TeamID")),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLPlayer), "TeamID")),
                new CodeInstruction(OpCodes.Call),
                new CodeInstruction(OpCodes.Beq_S)
            }; // playerContext.MyCurrentTLI.MyShipInfo.TeamID != playerContext.TeamID

            int LabelIndex = FindSequence(instructions, targetSequence, CheckMode.NONNULL) - 1;
            List<CodeInstruction> patchSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(BotPriorityPatches), "OnHostileShipPatch")),
                new CodeInstruction(OpCodes.Brtrue, instructions.ToList()[LabelIndex].operand)
            };

            return PatchBySequence(instructions, targetSequence, patchSequence, PatchMode.REPLACE, CheckMode.NONNULL);
        }
        static bool OnHostileShipPatch(PLPlayer playerContext)
        => playerContext.MyCurrentTLI.MyShipInfo.GetIsPlayerShip();
    }
}
