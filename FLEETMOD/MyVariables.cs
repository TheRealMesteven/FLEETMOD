using System;

namespace FLEETMOD
{
    internal class MyVariables
    {
        public static bool isrunningmod = true;
        public static bool shipfriendlyfire = false;
        public static bool shipgodmode = false;
        //public static float warprange = -1;
        public static int shipcount = 255;
        public static bool recentfriendlyfire = false;
        public static bool DialogGenerated = false;
        public static int GetShipCaptain (int inShipID)
        {
            foreach (PLPlayer pLPlayer in PLServer.Instance.AllPlayers)
            {
                if (pLPlayer != null && pLPlayer.GetClassID() == 0 && pLPlayer.TeamID == 0)
                {
                    if (pLPlayer.GetPhotonPlayer().GetScore() == inShipID)
                    {
                        return pLPlayer.GetPlayerID();
                    }
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
