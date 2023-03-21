using PulsarModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD.ModMessages
{
    public class ActivateFleetmod : ModMessage
    {
        public static List<PhotonPlayer> PhotonClients;
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (sender.sender == PhotonNetwork.masterClient)
            {
                MyVariables.isrunningmod = true;
            }
        }
    }
}
