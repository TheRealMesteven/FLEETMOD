using FLEETMOD.Bot;
using PulsarModLoader;
using System.Linq;

namespace FLEETMOD.ModMessages
{
    internal class ChangeShip : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (PhotonNetwork.isMasterClient && arguments[0] is int)
            {
                PLPlayer player = PLServer.GetPlayerForPhotonPlayer(sender.sender);
                if (arguments.Count() > 1 && arguments[1] is int) Variables.ChangeShip(player.GetPlayerID(), (int)arguments[0], (int)arguments[1]);
                else Variables.ChangeShip(player.GetPlayerID(), (int)arguments[0]);
            }
        }
    }
}
