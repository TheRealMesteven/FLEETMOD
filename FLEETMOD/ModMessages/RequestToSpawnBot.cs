using FLEETMOD.Bot;
using PulsarModLoader;

namespace FLEETMOD.ModMessages
{
    internal class RequestToSpawnBot : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (PhotonNetwork.isMasterClient && arguments[0] is int)
            {
                if (ServerAddCrewBotPlayer.AbleToAddBotPlayer(sender.sender, (int)arguments[0], out int PlayerID)) ServerAddCrewBotPlayer.AddCrewBotPlayer(PLServer.GetPlayerForPhotonPlayer(sender.sender), PlayerID, (int)arguments[0]);
            }
        }
    }
}
