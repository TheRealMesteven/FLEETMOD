using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLTabMenu), "Update")]
	internal class UpdatePLTabMenu
	{
		public static void Postfix(PLTabMenu __instance)
		{
			if (MyVariables.isrunningmod)
			{
				if (__instance != null && PLEncounterManager.Instance.PlayerShip != null && PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLNetworkManager.Instance.LocalPlayer.PlayerLifeTime > 3f)
				{
                    List<PLShipInfoBase> list = new List<PLShipInfoBase>();
					foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
					{
						if (plshipInfoBase.TagID == -23 && plshipInfoBase != null)
						{
							list.Add(plshipInfoBase);
                        }
					}
					if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.Comma) && PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp > 0 && __instance.TabMenuActive)
					{
						PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
						PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp = PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp - 1;
					}
					if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.Period) && PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp < list.Count - 1 && __instance.TabMenuActive)
					{
						PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
						PLPlayer localPlayer = PLNetworkManager.Instance.LocalPlayer;
						localPlayer.FBBiscuitsSoldSinceWarp++;
					}
                    List<PLPlayer> list2 = new List<PLPlayer>();
                    for (int i = 0; i < PLServer.Instance.AllPlayers.Count; i++)
					{
                        PLPlayer plplayer = PLServer.Instance.AllPlayers[i];
                        if (plplayer != null && plplayer.TeamID == 0 && plplayer.StartingShip != null && plplayer.StartingShip == list[PLNetworkManager.Instance.LocalPlayer.FBBiscuitsSoldSinceWarp])
						{
                            list2.Add(plplayer);
                        }
					}
                    list2.Sort(delegate(PLPlayer p1, PLPlayer p2)
					{
						int num2 = p1.GetClassID();
						int num3 = p2.GetClassID();
						if (num2 == -1)
						{
							num2 = 6 + p1.GetPlayerID();
						}
						if (num3 == -1)
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
						if (list2.Count > num)
						{
							plplayer2 = list2[num];
						}
						if (plplayer2 != null)
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
