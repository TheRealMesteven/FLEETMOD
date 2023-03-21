using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Setup
{
	[HarmonyPatch(typeof(PLUIPlayMenu), "GetJoinGameElementFromRoomInfo")]
	internal class GetJoinGameElementFromRoomInfo
	{
		public static bool Prefix(RoomInfo roomInfo, ref List<PLUIPlayMenu.UIJoinGameElement> ___m_JoinGameElements, ref PLUIPlayMenu.UIJoinGameElement __result)
		{
			if (PhotonNetwork.player.GetScore() > 0)
			{
				PhotonNetwork.player.SetScore(0);
			}
			foreach (PLUIPlayMenu.UIJoinGameElement uijoinGameElement in ___m_JoinGameElements)
			{
				if (roomInfo.CustomProperties["Ship_Type"].ToString().Contains("FLEETMOD") && uijoinGameElement.Room.Name == roomInfo.Name)
				{
					uijoinGameElement.GalaxySettingsLabel.text = (string)roomInfo.CustomProperties["Ship_Type"];
					uijoinGameElement.GalaxySettingsLabel.color = Color.yellow;
					uijoinGameElement.GalaxySettingsLabel.fontSize = 14;
				}
				if (uijoinGameElement.Room.Name == roomInfo.Name)
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
