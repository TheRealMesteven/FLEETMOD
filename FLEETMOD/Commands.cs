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

        public class FLEETMODShipRequest : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "shipreq"
                };
            }
            public string Description()
            {
                return "Fleetmod ship spawn request";
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
                        var go = new UnityEngine.GameObject("dummy"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                        var a = go.AddComponent<Interface.Dialogs.ShipSpawnRequest>(); // Also TODO: Rename local vars...
                        UnityEngine.GameObject.DontDestroyOnLoad(go);
                        PulsarPluginLoader.Utilities.Messaging.Notification("Check your dialogue screen!");
                    }
                }
                return false;
            }
        }

        public class FLEETMODFleetDisplay : IChatCommand
        {
            public string[] CommandAliases()
            {
                return new string[]
                {
                    "fleetreq"
                };
            }
            public string Description()
            {
                return "Fleetmod ship list request";
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
                        PulsarPluginLoader.Utilities.Messaging.Notification(Global.GetFleetShipCount().ToString());
                    }
                }
                return false;
            }
        }
    }
}
