﻿using System;
using PulsarPluginLoader;
using UnityEngine;

namespace FLEETMOD
{
    internal class ServerUpdateVariables : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            return; // *Broken Original disable
            MyVariables.shipfriendlyfire = (bool)arguments[0];
            //MyVariables.warprange = (float)arguments[1];
        }
    }
}
