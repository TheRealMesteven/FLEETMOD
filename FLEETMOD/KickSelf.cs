using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000023 RID: 35
	[HarmonyPatch(typeof(PLPlayer), "KickSelf")]
	internal class KickSelf
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00006608 File Offset: 0x00004808
		public static bool Prefix(PhotonMessageInfo pmi)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool isMasterClient = pmi.sender.IsMasterClient;
				if (isMasterClient)
				{
					PLNetworkManager.Instance.StartCoroutine(PLNetworkManager.Instance.AddErrorMsgMenu("You have been kicked from the game!", 1f));
					PLLoader.Instance.IsWaitingOnNetwork = true;
					PLNetworkManager.Instance.StartCoroutine("DelayedLeaveGame", 0.5f);
				}
				result = false;
			}
			return result;
		}
	}
}
