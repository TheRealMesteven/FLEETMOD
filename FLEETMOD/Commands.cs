using PulsarPluginLoader.Chat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD
{
    class Commands
    {
        public class FLEETMODShipLimit : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "shiplimit"
                };
            }
            public string Description()
            {
                return "Fleetmod Ship limit";
            }
            public string UsageExample()
            {
                return "/" + this.CommandAliases()[0];
            }
            public bool PublicCommand()
            {
                return false;
            }
            public bool Execute(string arguments, int SenderID)
            {
                if (Global.isrunningmod)
                {
                    if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
                    {
                        Global.shiplimit = Int32.Parse(arguments);
                        PLNetworkManager.Instance.ConsoleText.Insert(0, "Ship Limit Set To " + Global.shiplimit);
                        PulsarPluginLoader.Utilities.Messaging.Notification("FLEETMOD | Ship Limit Set To:" + Global.shiplimit + "\nRemember -1 removes the limit");
                    }
                }
                return false;
            }
        }
    }
}
