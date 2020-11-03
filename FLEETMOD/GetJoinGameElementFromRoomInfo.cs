using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x0200001C RID: 28
	[HarmonyPatch(typeof(PLUIPlayMenu), "GetJoinGameElementFromRoomInfo")]
	internal class GetJoinGameElementFromRoomInfo
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00005934 File Offset: 0x00003B34
		public static bool Prefix(RoomInfo roomInfo, ref List<PLUIPlayMenu.UIJoinGameElement> ___m_JoinGameElements, ref PLUIPlayMenu.UIJoinGameElement __result)
		{
			bool flag = PhotonNetwork.player.GetScore() > 0;
			if (flag)
			{
				PhotonNetwork.player.SetScore(0);
			}
			foreach (PLUIPlayMenu.UIJoinGameElement uijoinGameElement in ___m_JoinGameElements)
			{
				bool flag2 = roomInfo.CustomProperties["Ship_Type"].ToString().Contains("FLEETMOD") && uijoinGameElement.Room.Name == roomInfo.Name;
				if (flag2)
				{
					uijoinGameElement.GalaxySettingsLabel.text = (string)roomInfo.CustomProperties["Ship_Type"];
					uijoinGameElement.GalaxySettingsLabel.color = Color.blue;
					uijoinGameElement.GalaxySettingsLabel.fontSize = 14;
				}
				bool flag3 = uijoinGameElement.Room.Name == roomInfo.Name;
				if (flag3)
				{
					uijoinGameElement.Players.fontSize = 20;
					__result = uijoinGameElement;
					break;
				}
			}
			return false;
		}
	}
}
