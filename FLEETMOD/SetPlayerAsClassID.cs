using System;
using HarmonyLib;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLServer), "SetPlayerAsClassID")]
    internal class SetPlayerAsClassID
    {
        public static bool Prefix(int playerID, int classID)
        {
            if (!MyVariables.isrunningmod) return true;
            PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(playerID);
            if (playerFromPlayerID != null)
            {
                bool flag3 = false;
                foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
                {
                    if (plplayer != null && plplayer.TeamID == 0 && plplayer.GetClassID() == classID && plplayer != playerFromPlayerID && playerFromPlayerID.GetPhotonPlayer().GetScore() == 0)
                    {
                        flag3 = true;
                    }
                }
                if (!flag3)
                {
                    playerFromPlayerID.SetClassID(classID);
                }
            }
            return false;
        }
    }
}
