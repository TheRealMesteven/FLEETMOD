using System;
using PulsarPluginLoader;

namespace FLEETMOD
{
	internal class ServerChangePlayerNames : ModMessage
	{
		public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
		{
            return ; // *Broken Original disable
            foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
			{
				if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.GetPlayerName(false).Contains("•") && plplayer.GetPhotonPlayer().GetScore() == (int)arguments[1])
				{
					int startIndex = plplayer.GetPlayerName(false).LastIndexOf("•");
					plplayer.photonView.RPC("SetServerPlayerName", PhotonTargets.All, new object[]
					{
						(string)arguments[0] + " " + plplayer.GetPlayerName(false).Substring(startIndex)
					});
					PLServer.Instance.photonView.RPC("AddNotification", plplayer.GetPhotonPlayer(), new object[]
					{
						"Your ship has been renamed by your Captain",
						plplayer.GetPlayerID(),
						PLServer.Instance.GetEstimatedServerMs() + 3000,
						true
					});
				}
			}
		}
	}
}
