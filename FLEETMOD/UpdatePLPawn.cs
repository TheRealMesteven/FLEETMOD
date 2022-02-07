using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLPawn),"Update")]
    internal class UpdatePLPawn
    {

		// This fixes the health issue with death of a player that is the same class
		// Although rejoining resets max health to base health
        static void Postfix(PLPawn __instance)
        {
		
				if (__instance.GetPlayer() != null && __instance.GetPlayer().GetPlayerID() != -1)
                {
					MyVariables.MySurvivalBonus = MyVariables.survivalBonusDict[__instance.GetPlayer().GetPlayerID()];
						float num10 = 100f;
						if (__instance.GetPlayer().RaceID == 2)
						{
							num10 = 60f;
						}
						float num11 = num10 + (float)__instance.GetPlayer().Talents[0] * 20f;
						num11 += (float)__instance.GetPlayer().Talents[57] * 20f;
						foreach (PawnStatusEffect pawnStatusEffect5 in __instance.MyStatusEffects)
						{
							if (pawnStatusEffect5 != null && pawnStatusEffect5.Type == EPawnStatusEffectType.HEALTH_REGEN)
							{
								num11 += 20f;
							}
						}
						float value2 = num11;
						if (__instance.GetPlayer().GetClassID() < 5)
						{						
							num11 +=  MyVariables.MySurvivalBonus * 5f;
						}
						if (__instance.MaxHealth != num11)
						{
							__instance.Health = __instance.Health / __instance.MaxHealth * num11;
							__instance.MaxHealth = num11;
							__instance.MaxHealth_Normal = value2;
						}
				}
			
			

		}
    }
}
