using System;
using PulsarPluginLoader;

namespace FLEETMOD
{
	// Token: 0x02000026 RID: 38
	internal class ServerChangePlayerNames : ModMessage
	{
		// Token: 0x0600004A RID: 74 RVA: 0x000068AC File Offset: 0x00004AAC
		public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
		{
			foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
			{
				bool flag = plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.GetPlayerName(false).Contains("•") && plplayer.GetPhotonPlayer().GetScore() == (int)arguments[1];
				if (flag)
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
