using PulsarPluginLoader;
using System.Collections.Generic;

namespace FLEETMOD.ModMessages
{
    class SyncFleetIDs : ModMessage
    {
        //Used to Sync Global.Fleet with Masterclient's Fleet
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if(sender.sender == PhotonNetwork.masterClient)
            {
                Global.Fleet = (Dictionary<int, int>)arguments[0];
            }
        }
    }
}
