using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLWarpDriveScreen), "OnButtonClick")]
	internal class OnButtonClick
	{
		public static bool Prefix(UIWidget inButton)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				string name = inButton.name;
				bool flag2 = !PhotonNetwork.isMasterClient && !(name == "Jump");
				result = !flag2;
			}
			return result;
		}
	}
}
