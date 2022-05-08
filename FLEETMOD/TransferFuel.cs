using PulsarModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD
{
    class TransferFuel : ModMessage
    {
        public override void HandleRPC(object[] arguments /* int FromCrewKey, int ToShipKey, int Amount */, PhotonMessageInfo sender)
        {
            try
            {
                int amount = (int)arguments[2];
                PLEncounterManager.Instance.GetShipFromID((int)arguments[0]).NumberOfFuelCapsules -= amount;
                PLEncounterManager.Instance.GetShipFromID((int)arguments[1]).NumberOfFuelCapsules += amount;
            }
            catch (Exception e)
            {
                PulsarModLoader.Utilities.Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"ModMessages.FuelTransfer exception!\n{e.Message}");
            }
        }
    }
}
