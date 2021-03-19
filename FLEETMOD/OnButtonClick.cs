using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLWarpDriveScreen), "OnButtonClick")]
	internal class OnButtonClick
	{
		public static bool Prefix(UIWidget inButton)
		{
            return true; // *Broken Original disable
            if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				return !(!PhotonNetwork.isMasterClient && !(inButton.name == "Jump")); // Can probably be simplified but I don't want to mess it up without knowing logic
			}
		}
	}
}
