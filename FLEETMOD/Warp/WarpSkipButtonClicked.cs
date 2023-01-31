using System;
using HarmonyLib;

namespace FLEETMOD.Warp
{
    [HarmonyPatch(typeof(PLInGameUI), "WarpSkipButtonClicked")]
    internal class WarpSkipButtonClicked
    {
        public static bool Prefix()
        {
            if (!MyVariables.isrunningmod) return true;
            if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
            {
                PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.photonView.RPC("SkipWarp", PhotonTargets.All, Array.Empty<object>());
            }
            return false;
        }
    }
}
