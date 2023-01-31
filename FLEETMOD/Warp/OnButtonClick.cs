using System;
using HarmonyLib;

namespace FLEETMOD.Warp
{
	[HarmonyPatch(typeof(PLWarpDriveScreen), "OnButtonClick")]
	internal class OnButtonClick
	{
		public static bool Prefix(UIWidget inButton)
		{
            // Only permit Admiral to Click BlindJump / Self Destruct button
            if (!MyVariables.isrunningmod) return true;
			return !(!PhotonNetwork.isMasterClient && !(inButton.name == "Jump"));
		}
	}
}
