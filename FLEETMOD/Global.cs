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
        // Boolean check if ship initialised as fleet ship
        public static bool ishostshipsetup = false;
        // Dictionary linking Crew IDs to ShipIDs owned by the fleet.
        public static Dictionary<int, int> Fleet;
        // Dictionary Linking Player IDs and Crew IDs
        public static Dictionary<int, int> PlayerCrewList;
        // Shiplimit for playership spawning.
        public static int shiplimit = 5; 
        // Set to 5 for default testing, in release should be 1
        // Developer mode Check
        public static bool devmode = true;

        /// <summary>
        /// Checks if ship at ship id is in the fleet dictionary.
        /// </summary>
        /// <param name="inShipID">ShipID of ship</param>
        /// <returns>returns true if ship is in the fleet</returns>
        public static bool GetIsFleetShip(int inShipID)
        {
            foreach (int _ShipID in Fleet.Values)
            {
                if (_ShipID == inShipID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets Crew ID from Player ID
        /// Could've used the return function. However, this makes the code more readable IMO
        /// </summary>
        /// <param name="inPlayerID">Player ID of player</param>
        /// <returns>returns Players Crew ID</returns>
        public static int GetCrewID(int inPlayerID)
        {
            return PlayerCrewList[inPlayerID];
        }

        /// <summary>
        /// Gets Ship ID from Crew ID
        /// Could've used the return function. However, this makes the code more readable IMO
        /// </summary>
        /// <param name="inCrewID">Crew ID of player</param>
        /// <returns>returns Crews Ship ID</returns>
        public static int GetShipID(int inCrewID)
        {
            return Fleet[inCrewID];
        }

        /// <summary>
        /// Gets a count of all ships in the fleet
        /// </summary>
        /// <param name="num">Count of progress through fleet crew's</param>
        /// <returns>returns count of all ships in fleet</returns>
        public static int GetFleetShipCount()
        {
            int num = 0;
            foreach (int CrewID in Fleet.Keys)
            {
                num++;
            }
            return num;
        }

        /// <summary>
        /// Gets the lowest available crew id of the fleet
        /// </summary>
        /// <param name="i">Count of progress through fleet check</param>
        /// <returns>returns lowest Crew ID available</returns>
        public static int GetLowestUncrewedID()
        {
            //limit set to 1000, should never reach that
            for (int i = 1; i < 1000; i++)
            {
                if(!Fleet.ContainsKey(i))
                {
                    return i;
                }
            }
            throw new Exception("[FM] Something broke at FleetMod.Global.GetLowestUncrewedID");
        }
    }
}
