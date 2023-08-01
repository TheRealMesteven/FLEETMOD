using HarmonyLib;
using static PulsarModLoader.Patches.HarmonyHelpers;
using static HarmonyLib.AccessTools;
using System.Collections.Generic;
using System.Reflection.Emit;
using FLEETMOD.Ships;

namespace FLEETMOD.Interface
{
    [HarmonyPatch(typeof(PLTeleportationScreen), "UpdateDetectedObjectList")]
    class TeleporterPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return PatchBySequence(instructions,
            new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLLevelSync), "PlayerShip")),
            },
            new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Call, Method(typeof(TeleporterPatch), "PatchShip")),
            }, PatchMode.REPLACE, CheckMode.ALWAYS);
        }
        public static PLShipInfo PatchShip()
        {
            if (PLNetworkManager.Instance == null || PLNetworkManager.Instance.LocalPlayer == null) return PLEncounterManager.Instance.PlayerShip;
            return PLNetworkManager.Instance.LocalPlayer.StartingShip;
        }
    }
}
