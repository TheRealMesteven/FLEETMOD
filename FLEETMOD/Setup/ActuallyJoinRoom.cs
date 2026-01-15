using FLEETMOD.Interface.Tab;
using HarmonyLib;
using System.Linq;

namespace FLEETMOD.Setup
{
    [HarmonyPatch(typeof(PLUIPlayMenu), "ActuallyJoinRoom")]
    internal class ActuallyJoinRoom
    {
        /// <summary>
        /// Used to update play-list with fleet name.
        /// Used to trigger Fleet.
        /// </summary>
        public static bool Prefix(RoomInfo room)
        {
            if ((int)room.CustomProperties["CurrentPlayersPlusBots"] < (int)room.MaxPlayers)
            {
                if (room.CustomProperties.ContainsKey("SteamServerID"))
                {
                    if (!SteamManager.Initialized)
                    {
                        PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Failed to join crew! Can't join Secured game when not logged in to Steam!"));
                        return false;
                    }
                    uint num = (uint)((long)room.CustomProperties["SteamServerID"]);
                    PLNetworkManager.Instance.ClearSteamAuthSession(num);
                    PLNetworkManager.Instance.SetSteamAuthTicket(num);
                }
                if ((string)room.CustomProperties["Ship_Type"] == Mod.myversion)
                {
                    Variables.isrunningmod = false;
                    PLNetworkManager.Instance.JoinRoom(room);
                    PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
                    PLLoader.Instance.IsWaitingOnNetwork = true;
                }
                if (!room.CustomProperties["Ship_Type"].ToString().Contains("FLEETMOD"))
                {
                    Variables.isrunningmod = false;
                    PLNetworkManager.Instance.JoinRoom(room);
                    PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
                    PLLoader.Instance.IsWaitingOnNetwork = true;
                }
                if ((string)room.CustomProperties["Ship_Type"] != Mod.myversion && room.CustomProperties["Ship_Type"].ToString().Contains("."))
                {
                    PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Sorry, This Server Is Running FleetMod Version " + (string)room.CustomProperties["Ship_Type"] + "\n You Have Version " + Mod.myversion));
                }
                return false;
            }
            else
            {
                PLTabMenu.Instance.TimedErrorMsg = "Couldn't join crew! It is full!";
                return false;
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "NotifyPlayerStart")]
    internal class NotifyPlayerStart // Called whenever pawn is spawned.
    {
        private static void Postfix(PLServer __instance, int inPlayerID)
        {
            PLPlayer playerAtID = __instance.GetPlayerFromPlayerID(inPlayerID);
            if (playerAtID != null && PhotonNetwork.isMasterClient && !(Variables.Modded.Contains(inPlayerID) || Variables.NonModded.Contains(inPlayerID) || playerAtID.IsBot))
            {
                if (playerAtID == PLNetworkManager.Instance.LocalPlayer) Variables.Modded.Add(inPlayerID);
                else if (PulsarModLoader.MPModChecks.MPModCheckManager.Instance.NetworkedPeerHasMod(playerAtID.GetPhotonPlayer(), Mod.harmonyIden)) Variables.Modded.Add(inPlayerID);
                else
                {
                    Variables.NonModded.Add(inPlayerID);
                }
                int shipToAssign = Variables.Fleet.Keys.ToList().FirstOrDefault();
                Variables.Fleet[shipToAssign].Add(inPlayerID);
                playerAtID.GetPhotonPlayer().SetScore(shipToAssign);
                ModMessages.ServerUpdateVariables.UpdateClients();
            }
        }
    }
    [HarmonyPatch(typeof(PLGame), "Start")]
    internal class TriggerFleetmod
    {
        private static void Postfix()
        {
            if (PhotonNetwork.isMasterClient)
            {
                Variables.isrunningmod = true;
            }
            else
            {
                ChangeTabMenuDisplay.Postfix(); // Used to start the ChangeTabMenu alterations for non-masterclient.
            }
        }
    }
}
