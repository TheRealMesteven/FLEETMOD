using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "Update")]
	internal class UpdatePlayerOverview
	{
		// Token: 0x06000013 RID: 19 RVA: 0x0000291C File Offset: 0x00000B1C
		public static void Postfix(PLOverviewPlayerInfoDisplay __instance, ref float ___cached_LastUpdatedPlayerInfoTime, ref PLPlayer ___cached_DisplayedPlayer, ref int ___cached_DisplayedPlayerClass, ref float ___cached_DisplayedPlayerHealth, ref bool ___cached_DisplayedPlayerIsTalking)
		{
			bool isrunningmod = MyVariables.isrunningmod;
			if (isrunningmod)
			{
				bool flag = __instance.MyPlayer != null && __instance.MyPlayer.GetPhotonPlayer().IsMasterClient && __instance.MyPlayer.GetClassID() == 0;
				if (flag)
				{
					PLGlobal.SafeLabelSetText(__instance.ClassName, "<color=#ffff00>THE ADMIRAL</color>");
				}
				bool flag2 = __instance.MyPlayer != null && !__instance.MyPlayer.GetPhotonPlayer().IsMasterClient && __instance.MyPlayer.GetClassID() == 0;
				if (flag2)
				{
					PLGlobal.SafeLabelSetText(__instance.ClassName, "<color=#0066FF>THE CAPTAIN</color>");
				}
				int num = (__instance.MyPlayer != null) ? __instance.MyPlayer.GetClassID() : -1;
				float num2 = 0f;
				bool flag3 = __instance.MyPlayer != null && __instance.MyPlayer.TS_IsTalking;
				bool flag4 = PLServer.Instance != null && PLServer.Instance.LocalCachedPlayerByClass_LastChangedTime > ___cached_LastUpdatedPlayerInfoTime;
				if (flag4)
				{
					___cached_LastUpdatedPlayerInfoTime = 0f;
				}
				bool flag5 = __instance.MyPlayer != ___cached_DisplayedPlayer || ___cached_DisplayedPlayerClass != num || Time.time - ___cached_LastUpdatedPlayerInfoTime > UnityEngine.Random.Range(0.7f, 2f) || ___cached_DisplayedPlayerHealth != num2 || ___cached_DisplayedPlayerIsTalking != flag3;
				if (flag5)
				{
					___cached_DisplayedPlayer = __instance.MyPlayer;
					___cached_LastUpdatedPlayerInfoTime = Time.time;
					___cached_DisplayedPlayerHealth = num2;
					___cached_DisplayedPlayerIsTalking = flag3;
					bool flag6 = __instance.MyPlayer == null;
					if (flag6)
					{
						PLGlobal.SafeLabelSetText(__instance.PlayerName, "Role Available");
						bool flag7 = ___cached_DisplayedPlayerClass != num;
						if (flag7)
						{
							PLGlobal.SafeLabelSetText(__instance.ClassName, "");
							___cached_DisplayedPlayerClass = num;
						}
						__instance.BG.color = Color.gray * 0.1f;
					}
					else
					{
						__instance.PlayerName.text = PLReadableStringManager.Instance.GetFormattedResultFromInputString(__instance.MyPlayer.GetPlayerName(true));
						bool flag8 = ___cached_DisplayedPlayerClass != num;
						if (flag8)
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
						bool flag9 = __instance.MyPlayer.GetClassID() != -1;
						if (flag9)
						{
							color = PLGlobal.Instance.ClassColors[__instance.MyPlayer.GetClassID()];
						}
						else
						{
							color = Color.gray;
						}
						__instance.BG.color = color * 0.5f;
						__instance.ClassName.color = color;
						bool flag10 = __instance.MyPlayer.GetClassID() == 0;
						if (flag10)
						{
							PLNetworkManager.Instance.LocalPlayer.ArenaScore = __instance.MyPlayer.StartingShip.ShipID;
						}
					}
				}
			}
		}
	}
}
