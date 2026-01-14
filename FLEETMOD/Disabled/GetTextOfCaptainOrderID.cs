using FLEETMOD.Ships;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static HarmonyLib.AccessTools;
using static PulsarModLoader.Patches.HarmonyHelpers;

namespace FLEETMOD.Disabled
{
	[HarmonyPatch(typeof(PLInGameUI), "GetTextOfCaptainOrderID")]
	internal class GetTextOfCaptainOrderID
	{
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> target = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLInGameUI), "CurrentOrdersLabel")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLServer), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLServer), "CaptainOrdersID")),
                new CodeInstruction(OpCodes.Call),
                new CodeInstruction(OpCodes.Call, Method(typeof(PLInGameUI), "GetTextOfCaptainOrderID", new Type[] { typeof(Int32) })),
                new CodeInstruction(OpCodes.Callvirt)
            }; //this.CurrentOrdersLabel.text = this.GetTextOfCaptainOrderID(PLServer.Instance.CaptainsOrdersID);
            return PatchBySequence(instructions, target, new List<CodeInstruction>(), PatchMode.REPLACE, showDebugOutput: false);
        }
    }
}
