using PulsarPluginLoader;
using System.Collections.Generic;

namespace FLEETMOD.ModMessages
{
    class ServerAssignShip : ModMessage
    {
        //Used to Sync Global.Fleet with Masterclient's Fleet
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            // arguments[0] == CrewID
            if (!(PhotonNetwork.isMasterClient))
            {
                try
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FM] Ship Assigning");
                    PulsarPluginLoader.Utilities.Logger.Info("[FM] 2 " + Global.GetShipIDFromCrewID((int)arguments[0]).ToString());
                    PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID = Global.GetShipIDFromCrewID((int)arguments[0]);
                    PulsarPluginLoader.Utilities.Logger.Info("[FM] 3 " + PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID);
                }
                catch
                {
                    PulsarPluginLoader.Utilities.Logger.Info("[FM] Ship Assignment Failure!");
                    PulsarPluginLoader.Utilities.Logger.Info("[FM] " + PLEncounterManager.Instance.GetShipFromID(Global.Fleet[(int)arguments[0]]).ShipName);
                }
            }
        }
    }
}
