using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD
{
    internal class Global
    {
        // Boolean check if the mod is running
        public static bool isrunningmod;
        // Dictionary linking Ship IDs and Crew IDs owned by the fleet.
        public static Dictionary<int, int> Fleet;
        // Dictionary Linking Player IDs and Crew IDs
        public static Dictionary<int, int> PlayerCrewList;
        // Shiplimit for playership spawning.
        public static int shiplimit = 1;
        // Developer mode Check
        public static bool devmode = true;

        /// <summary>
        /// Checks if ship at ship id is in the fleet dictionary.
        /// </summary>
        /// <param name="inShipID">ShipID of ship</param>
        /// <returns>returns true if ship is in the fleet</returns>
        public static bool GetIsFleetShip(int inShipID)
        {
            foreach (int shipID in Fleet.Keys)
            {
                if (inShipID == shipID)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
