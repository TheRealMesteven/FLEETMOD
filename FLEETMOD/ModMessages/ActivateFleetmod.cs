using PulsarModLoader;
using System.Collections.Generic;

namespace FLEETMOD.ModMessages
{
    public class ActivateFleetmod : ModMessage
    {
        public static List<PhotonPlayer> PhotonClients;
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (sender.sender == PhotonNetwork.masterClient)
            {
                Variables.isrunningmod = true;
            }
        }
    }
}
