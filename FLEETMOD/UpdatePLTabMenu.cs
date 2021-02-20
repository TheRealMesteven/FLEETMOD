using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000013 RID: 19
	[HarmonyPatch(typeof(PLTabMenu), "Update")]
	internal class UpdatePLTabMenu
	{
		// Token: 0x06000021 RID: 33 RVA: 0x00004A6C File Offset: 0x00002C6C
		public static void Postfix(PLTabMenu __instance)
		{
			bool isrunningmod = MyVariables.isrunningmod;
			if (isrunningmod)
			{
                bool flag = __instance != null && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLNetworkManager.Instance.LocalPlayer.PlayerLifeTime > 3f;
				if (flag)
				{
                    List<PLShipInfoBase> list = new List<PLShipInfoBase>();
					foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
					{
						bool flag2 = plshipInfoBase.TagID == -23 && plshipInfoBase != null;
						if (flag2)
						{
							list.Add(plshipInfoBase);
                        }
					}
					bool flag3 = !PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.Comma) && PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp > 0 && __instance.TabMenuActive;
					if (flag3)
					{
						PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
						PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp = PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp - 1;
					}
                    bool flag4 = !PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.Period) && PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp < list.Count - 1 && __instance.TabMenuActive;
					if (flag4)
					{
						PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
						PLPlayer localPlayer = PLNetworkManager.Instance.LocalPlayer;
						localPlayer.FBBiscuitsSoldSinceWarp++;
					}
                    List<PLPlayer> list2 = new List<PLPlayer>();;
                    for (int i = 0; i < PLServer.Instance.AllPlayers.Count; i++)
					{
                        PLPlayer plplayer = PLServer.Instance.AllPlayers[i];
                        bool flag5 = plplayer != null && plplayer.TeamID == 0 && plplayer.StartingShip != null && plplayer.StartingShip == list[PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp];
                        if (flag5)
						{
                            list2.Add(plplayer);
                        }
					}
                    list2.Sort(delegate(PLPlayer p1, PLPlayer p2)
					{
						int num2 = p1.GetClassID();
						int num3 = p2.GetClassID();
						bool flag8 = num2 == -1;
						if (flag8)
						{
							num2 = 6 + p1.GetPlayerID();
						}
						bool flag9 = num3 == -1;
						if (flag9)
						{
							num3 = 6 + p2.GetPlayerID();
						}
						return num2.CompareTo(num3);
					});
                    int num = 0;
					for (int j = 0; j < __instance.PlayerInfoDisplays.Length; j++)
					{
						PLOverviewPlayerInfoDisplay ploverviewPlayerInfoDisplay = __instance.PlayerInfoDisplays[j];
						PLPlayer plplayer2 = null;
						bool flag6 = list2.Count > num;
						if (flag6)
						{
							plplayer2 = list2[num];
						}
						bool flag7 = plplayer2 != null;
						if (flag7)
						{
							ploverviewPlayerInfoDisplay.ClassID = plplayer2.GetClassID();
						}
						else
						{
							ploverviewPlayerInfoDisplay.ClassID = -1;
						}
						ploverviewPlayerInfoDisplay.MyPlayer = plplayer2;
						num++;
					}
                }
			}
		}
	}
}
