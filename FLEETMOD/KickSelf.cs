using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLPlayer), "KickSelf")]
	internal class KickSelf
	{
		public static bool Prefix(PhotonMessageInfo pmi)
		{
            return true; // *Broken Original disable
            if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (pmi.sender.IsMasterClient)
				{
					PLNetworkManager.Instance.StartCoroutine(PLNetworkManager.Instance.AddErrorMsgMenu("You have been kicked from the game!", 1f));
					PLLoader.Instance.IsWaitingOnNetwork = true;
					PLNetworkManager.Instance.StartCoroutine("DelayedLeaveGame", 0.5f);
				}
				return false;
			}
		}
	}
}
