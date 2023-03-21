using System;
using PulsarModLoader;

namespace FLEETMOD.ModMessages
{
	internal class ServerChangePlayerNames : ModMessage
	{
		public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
		{
			foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
			{
				if (plplayer != null && plplayer.GetPhotonPlayer() != null && plplayer.GetPhotonPlayer().GetScore() == (int)arguments[1])
				{
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
