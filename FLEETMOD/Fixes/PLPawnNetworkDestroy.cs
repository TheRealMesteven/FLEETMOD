using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Fixes
{
    [HarmonyPatch(typeof(PLPawn),"NetworkDestroy")]
    internal class PLPawnNetworkDestroy
    {
        public static void Postfix(PLPawn __instance)
        {

            if (PhotonNetwork.isMasterClient) // if player dies host updates that player's bonus
            {
                var playerid = __instance.GetPlayer().GetPlayerID();
                Variables.survivalBonusDict[playerid] = Mathf.Clamp(Variables.survivalBonusDict[playerid] - 5, -5, 20);     
            }
        }
    }
}
