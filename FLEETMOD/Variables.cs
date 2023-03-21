using System;
using System.Collections.Generic;
using System.Linq;

namespace FLEETMOD
{
    internal class Variables
    {
        public static bool isrunningmod = false;
        public static bool shipfriendlyfire = false;
        public static bool shipgodmode = false;
        public static int shipcount = 5;
        public static bool recentfriendlyfire = false;
        public static bool DialogGenerated = false;
        public static bool FuelDialog = false;
        public static bool CargoMenu = false;
        public static Dictionary<int, List<int>> Fleet; /*ShipID, List<PlayerID>*/
        public static List<int> Modded; // PlayerID of the Players who have Fleetmod active and running
        public static List<int> NonModded; // PlayerID of the Players who dont have Fleetmod active and running
        public static Dictionary<int /*PlayerID*/ , /*Bonus*/ int> survivalBonusDict; 
        // Dictionary that stores <playerID,healthBonus> on hostside, then it's being sent to clients
        public static int MySurvivalBonus;
        // variable for storing localplayer's healthBonus
        public static Dictionary<int /*PlayerID*/ , /*ShipID*/ int> UnModdedCrews;
        // Dictionary that stores <playerID,shipID> on host side, to teleport unmodded crews to correct ship.

        public static int GetShipCaptain (int inShipID)
        {
            foreach (PLPlayer Player in PLServer.Instance.AllPlayers)
            {
                if (Player != null && Player.TeamID == 0 && Player.GetPhotonPlayer().GetScore() == inShipID && Player.GetClassID() == 0)
                        return Player.GetPlayerID();
            }
            return -1;
        }
        public static bool ShipHasCaptain (int inShipID)
        {
            if (PLServer.Instance != null && PLEncounterManager.Instance.GetShipFromID(inShipID) != null)
            {
                if (GetShipCaptain(inShipID) != -1)
                {
                    return true;
                }
            }
            return false;
        }
        public static void ReCalculateMaxPlayers()
        {
            if (Fleet.Count() * 5 != PhotonNetwork.room.MaxPlayers)
            {
                PhotonNetwork.room.MaxPlayers = Fleet.Count() * 5;
            }
        }
    }
}
