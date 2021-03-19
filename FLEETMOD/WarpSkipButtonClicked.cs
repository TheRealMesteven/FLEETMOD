using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLInGameUI), "WarpSkipButtonClicked")]
	internal class WarpSkipButtonClicked
	{
		public static bool Prefix()
		{
            return true; // *Broken Original disable
            if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
				{
					PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.photonView.RPC("SkipWarp", PhotonTargets.All, Array.Empty<object>());
				}
				return false;
			}
		}
	}
}
