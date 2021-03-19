using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "Update")]
	internal class UpdatePlayerOverview
	{
		public static void Postfix(PLOverviewPlayerInfoDisplay __instance, ref float ___cached_LastUpdatedPlayerInfoTime, ref PLPlayer ___cached_DisplayedPlayer, ref int ___cached_DisplayedPlayerClass, ref float ___cached_DisplayedPlayerHealth, ref bool ___cached_DisplayedPlayerIsTalking)
		{
            return; // *Broken Original disable
            if (MyVariables.isrunningmod)
			{
				if (__instance.MyPlayer != null && __instance.MyPlayer.GetPhotonPlayer().IsMasterClient && __instance.MyPlayer.GetClassID() == 0)
				{
					PLGlobal.SafeLabelSetText(__instance.ClassName, "<color=#ffff00>THE ADMIRAL</color>");
				}
				if (__instance.MyPlayer != null && !__instance.MyPlayer.GetPhotonPlayer().IsMasterClient && __instance.MyPlayer.GetClassID() == 0)
				{
					PLGlobal.SafeLabelSetText(__instance.ClassName, "<color=#0066FF>THE CAPTAIN</color>");
				}
				int num = (__instance.MyPlayer != null) ? __instance.MyPlayer.GetClassID() : -1;
				float num2 = 0f;
				bool flag3 = __instance.MyPlayer != null && __instance.MyPlayer.TS_IsTalking;
				if (PLServer.Instance != null && PLServer.Instance.LocalCachedPlayerByClass_LastChangedTime > ___cached_LastUpdatedPlayerInfoTime)
				{
					___cached_LastUpdatedPlayerInfoTime = 0f;
				}
				if (__instance.MyPlayer != ___cached_DisplayedPlayer || ___cached_DisplayedPlayerClass != num || Time.time - ___cached_LastUpdatedPlayerInfoTime > UnityEngine.Random.Range(0.7f, 2f) || ___cached_DisplayedPlayerHealth != num2 || ___cached_DisplayedPlayerIsTalking != flag3)
				{
					___cached_DisplayedPlayer = __instance.MyPlayer;
					___cached_LastUpdatedPlayerInfoTime = Time.time;
					___cached_DisplayedPlayerHealth = num2;
					___cached_DisplayedPlayerIsTalking = flag3;
					if (__instance.MyPlayer == null)
					{
						PLGlobal.SafeLabelSetText(__instance.PlayerName, "Role Available");
						if ( ___cached_DisplayedPlayerClass != num)
						{
							PLGlobal.SafeLabelSetText(__instance.ClassName, "");
							___cached_DisplayedPlayerClass = num;
						}
						__instance.BG.color = Color.gray * 0.1f;
					}
					else
					{
						__instance.PlayerName.text = PLReadableStringManager.Instance.GetFormattedResultFromInputString(__instance.MyPlayer.GetPlayerName(true));
						if (___cached_DisplayedPlayerClass != num)
						{
							switch (num)
							{
							case 0:
								PLGlobal.SafeLabelSetText(__instance.ClassName, "THE CAPTAIN");
								break;
							case 1:
								PLGlobal.SafeLabelSetText(__instance.ClassName, "THE PILOT");
								break;
							case 2:
								PLGlobal.SafeLabelSetText(__instance.ClassName, "THE SCIENTIST");
								break;
							case 3:
								PLGlobal.SafeLabelSetText(__instance.ClassName, "THE WEAPONS SPECIALIST");
								break;
							case 4:
								PLGlobal.SafeLabelSetText(__instance.ClassName, "THE ENGINEER");
								break;
							default:
								PLGlobal.SafeLabelSetText(__instance.ClassName, "");
								break;
							}
							___cached_DisplayedPlayerClass = num;
						}
						Color color = Color.white;
						if (__instance.MyPlayer.GetClassID() != -1)
						{
							color = PLGlobal.Instance.ClassColors[__instance.MyPlayer.GetClassID()];
						}
						else
						{
							color = Color.gray;
						}
						__instance.BG.color = color * 0.5f;
						__instance.ClassName.color = color;
						if (__instance.MyPlayer.GetClassID() == 0)
						{
							PLNetworkManager.Instance.LocalPlayer.ArenaScore = __instance.MyPlayer.StartingShip.ShipID;
						}
					}
				}
			}
		}
	}
}
