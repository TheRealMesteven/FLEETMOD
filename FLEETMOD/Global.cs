﻿using System;
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
        // Dictionary linking Crew IDs to Ships owned by the fleet.
        public static Dictionary<int, PLShipInfo> Fleet;
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
            foreach (PLShipInfo _ShipInfo in Fleet.Values)
            {
                if (_ShipInfo == PLEncounterManager.Instance.GetShipFromID(inShipID))
                {
                    return true;
                }
            }
            return false;
        }
        public static int GetFleetShipCount()
        {
            int num = 0;
            foreach (int CrewID in Fleet.Keys)
            {
                num++;
            }
            return num;
        }
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