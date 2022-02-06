using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLPawn),"NetworkDestroy")]
    internal class PLPawnNetworkDestroy
    {
        public static void Postfix(PLPawn __instance)
        {

            if (__instance.GetPlayer().GetClassID() != -1 && __instance.GetPlayer().GetPlayerID() == PLNetworkManager.Instance.LocalPlayerID && __instance.GetPlayer().GetClassID() < 5 && __instance.GetPlayer().TeamID == 0)
            {
                MyVariables.survivalBonus -= 5;
                MyVariables.survivalBonus = Mathf.Clamp(MyVariables.survivalBonus, -5, 20);
            }
            
        }
    }
}
