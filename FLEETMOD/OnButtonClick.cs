using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000011 RID: 17
	[HarmonyPatch(typeof(PLWarpDriveScreen), "OnButtonClick")]
	internal class OnButtonClick
	{
		// Token: 0x0600001D RID: 29 RVA: 0x000047F8 File Offset: 0x000029F8
		public static bool Prefix(UIWidget inButton)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
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
