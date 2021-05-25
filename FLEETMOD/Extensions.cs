using System;
using static PulsarPluginLoader.Utilities.Logger;

namespace FLEETMOD
{
    public static class Extensions
    {
        public static PLShipInfoBase GetShip(this PLPlayer player)
        {
            try
            {
                return PLEncounterManager.Instance.GetShipFromID(Global.Fleet[Global.PlayerCrewList[PLNetworkManager.Instance.LocalPlayerID]]);
            }
            catch (Exception e)
            {
                Info($"Extensions::GetShip() exception: {e}");
                return null;
            }
        }
    }
}