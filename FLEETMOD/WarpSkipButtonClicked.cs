using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000019 RID: 25
	[HarmonyPatch(typeof(PLInGameUI), "WarpSkipButtonClicked")]
	internal class WarpSkipButtonClicked
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00005778 File Offset: 0x00003978
		public static bool Prefix()
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0;
				if (flag2)
				{
					PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.photonView.RPC("SkipWarp", PhotonTargets.All, Array.Empty<object>());
				}
				result = false;
			}
			return result;
		}
	}
}
