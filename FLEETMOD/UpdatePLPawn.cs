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
        static void Postfix(PLPawn __instance)
        {
			if (__instance.GetPlayer() != null)
			{
				float standardHealth = 100f;
				if (__instance.GetPlayer().RaceID == 2)
				{
					standardHealth = 60f;
				}
				float UpgradedHealth = standardHealth + (float)__instance.GetPlayer().Talents[0] * 20f;
				UpgradedHealth += (float)__instance.GetPlayer().Talents[57] * 20f;
				foreach (PawnStatusEffect pawnStatusEffect5 in __instance.MyStatusEffects)
				{
					if (pawnStatusEffect5 != null && pawnStatusEffect5.Type == EPawnStatusEffectType.HEALTH_REGEN)
					{
						UpgradedHealth += 20f;
					}
				}
				float value2 = UpgradedHealth;
				if (__instance.GetPlayer().GetClassID() != -1 && __instance.GetPlayer().GetClassID() < 5 && __instance.GetPlayer().TeamID == 0)
				{	
					UpgradedHealth += (float)MyVariables.survivalBonus * 5f;
				}
				if (__instance.MaxHealth != UpgradedHealth)
				{
					__instance.Health = __instance.Health / __instance.MaxHealth * UpgradedHealth;
					__instance.MaxHealth = UpgradedHealth;
					__instance.MaxHealth_Normal = value2;
				}
			}

		}
    }
}
