using FLEETMOD.Fixes;
using PulsarModLoader;

namespace FLEETMOD.ModMessages
{
    internal class RequestToSpawnBot : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (sender.sender == PhotonNetwork.masterClient && arguments[0] is int && arguments[1] is int) ServerAddCrewBotPlayer.AddCrewBotPlayer((int)arguments[0], (int)arguments[1]);
            else if (PhotonNetwork.isMasterClient && arguments[0] is int)
            {
                if (ServerAddCrewBotPlayer.AbleToAddBotPlayer(sender.sender, (int)arguments[0], out int PlayerID)) ModMessage.SendRPC(Mod.harmonyIden, "FLEETMOD.ModMessages.RequestToSpawnBot", sender.sender, new object[] { PlayerID, (int)arguments[0] });
            }
        }
    }
}
