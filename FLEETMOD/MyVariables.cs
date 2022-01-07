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
        public static bool recentfriendlyfire = false;
        public static bool DialogGenerated = false;
        public static bool CargoMenu = false;
        public static Dictionary<int, PLPlayer> ShipCrews;
        // ShipID, PLPlayer // List of crews in ship
        public static List<int> Fleet;
        // ShipIDs of the ships in the Fleet
        public static List<PLPlayer> EnabledFleetmod;
        // PLPlayer of the Players who have Fleetmod active and running
        
        public static int GetShipCaptain (int inShipID)
        {
            foreach (KeyValuePair<int,PLPlayer> pair in ShipCrews)
            {
                if (pair.Value != null && pair.Key == inShipID && pair.Value.GetClassID() == 0 && pair.Value.TeamID == 0)
                {
                    //if (pLPlayer.GetPhotonPlayer().GetScore() == inShipID)
                    //{
                        return pair.Value.GetPlayerID();
                    //}
                }
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
    }
}
