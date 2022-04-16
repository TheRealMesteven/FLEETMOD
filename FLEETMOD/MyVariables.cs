using System;
using System.Collections.Generic;
using System.Linq;

namespace FLEETMOD
{
    internal class MyVariables
    {
        public static bool isrunningmod = true;
        public static bool shipfriendlyfire = false;
        public static bool shipgodmode = false;
        //public static float warprange = -1;
        public static int shipcount = 7;
        // variable for storing current survival bonus. Used in UpdatePLPawn
        public static bool recentfriendlyfire = false;
        public static bool DialogGenerated = false;
        public static bool CargoMenu = false;
        public static Dictionary<PLShipInfo, int /*PlayerID*/> ShipCrews;
        // ShipID, PlayerID // List of crews in ship
        //public static List<PLShipInfo> Fleet;
        // ShipIDs of the ships in the Fleet
        //public static List<PhotonPlayer> FleetmodPhoton;
        //public static List<int /*PlayerID*/> FleetmodPlayer;
        // PlayerID of the Players who have Fleetmod active and running
        public static Dictionary<int /*PlayerID*/ , /*Bonus*/ int> survivalBonusDict; 
        // Dictionary that stores <playerID,healthBonus> on hostside, then it's being sent to clients
        public static int MySurvivalBonus;
        // variable for storing localplayer's healthBonus
        public static Dictionary<int /*PlayerID*/ , /*ShipID*/ int> UnModdedCrews;
        // Dictionary that stores <playerID,shipID> on host side, to teleport unmodded crews to correct ship.


        public static int GetShipCaptain (int inShipID)
        {
            foreach (KeyValuePair<PLShipInfo, int> pair in ShipCrews)
            {
                PLPlayer Player = PLServer.Instance.GetPlayerFromPlayerID(pair.Value);
                if (pair.Key == PLEncounterManager.Instance.GetShipFromID(inShipID) && Player.GetClassID() == 0 && Player.TeamID == 0)
                {
                        return pair.Value;
                }
            }
            return -1;
        }
        public static List<int> GetShipCrew (int inShipID)
        {
            List<int> Crew = null;
            foreach (KeyValuePair<PLShipInfo, int> pair in ShipCrews)
            {
                PLPlayer Player = PLServer.Instance.GetPlayerFromPlayerID(pair.Value);
                if (Player != null && pair.Key == PLEncounterManager.Instance.GetShipFromID(inShipID) && Player.TeamID == 0)
                {
                    Crew.Add(pair.Value);
                }
            }
            return Crew;
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
    }
}
