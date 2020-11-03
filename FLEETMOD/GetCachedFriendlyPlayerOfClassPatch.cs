using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000028 RID: 40
	[HarmonyPatch(typeof(PLServer), "GetCachedFriendlyPlayerOfClass", new Type[]
	{
		typeof(int)
	})]
	internal class GetCachedFriendlyPlayerOfClassPatch
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00006B4C File Offset: 0x00004D4C
		public static bool Prefix(PLServer __instance, ref int inClass)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = inClass == 0;
				if (flag2)
				{
					result = PLServer.Instance.GetPlayerFromPlayerID(0);
				}
				else
				{
					foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
					{
						bool flag3 = plplayer != null && !plplayer.IsBot && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.StartingShip != null && plplayer.StartingShip == PLNetworkManager.Instance.LocalPlayer.StartingShip && plplayer.StartingShip != null && plplayer.GetClassID() == inClass && inClass != 0 && __instance != null;
						if (flag3)
						{
							return plplayer;
						}
					}
					result = false;
				}
			}
			return result;
		}
	}
}
