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
        // Dictionary linking Crew IDs and Ship IDs owned by the fleet.
        public static Dictionary<int, int> Fleet;
        // Shiplimit for playership spawning.
        public static int shiplimit = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inShipID">ShipID from ship</param>
        /// <returns></returns>
        public static bool GetIsFriendlyShip(int inShipID)
        {
            foreach (int shipID in Fleet.Values)
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
