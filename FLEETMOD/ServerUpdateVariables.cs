using System;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD
{
    internal class ServerUpdateVariables : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            MyVariables.shipfriendlyfire = (bool)arguments[0];
            MyVariables.recentfriendlyfire = (bool)arguments[1];
            //MyVariables.warprange = (float)arguments[1];
        }
    }
}
