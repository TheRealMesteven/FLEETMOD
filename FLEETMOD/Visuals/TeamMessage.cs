using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Photon;
using PulsarModLoader.Patches;
using static RootMotion.FinalIK.InteractionObject;

namespace FLEETMOD.Visuals
{
	[HarmonyPatch(typeof(PLNetworkManager), "ProcessCurrentChatText")]
	internal class TeamMessage
	{
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return HarmonyHelpers.PatchBySequence(instructions, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldsfld),
                new CodeInstruction(OpCodes.Callvirt),
                new CodeInstruction(OpCodes.Ldstr),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Newarr),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLNetworkManager), "CurrentChatText")),
                new CodeInstruction(OpCodes.Stelem_Ref),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld),
                new CodeInstruction(OpCodes.Call),
                new CodeInstruction(OpCodes.Box),
                new CodeInstruction(OpCodes.Stelem_Ref),
                new CodeInstruction(OpCodes.Callvirt)
            },
            new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0, null),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TeamMessage), "Replacement"))
            },
            HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL, true);
		}
		private static void Replacement(PLNetworkManager __instance)
		{
			PLPlayer pLPlayer = __instance.LocalPlayer;
            string text = __instance.CurrentChatText.Replace("[&%~[C", "").Replace(" ]&%~]", "");
            PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, string.Concat(new object[]
            {
                "[&%~[C",
                $"{(PhotonNetwork.isMasterClient ? "5" : pLPlayer.GetClassID().ToString())}",
                $"{(PhotonNetwork.isMasterClient ? "  " : " ")}",
                (pLPlayer.StartingShip == null ? "" : (pLPlayer.StartingShip.ShipNameValue + " • ")) + pLPlayer.GetPlayerName(false),
                " <",
                $"{(PhotonNetwork.isMasterClient ? "Admiral" : pLPlayer.GetClassName())}",
                "> ]&%~]: ",
				text
			}));
        }
	}
}
