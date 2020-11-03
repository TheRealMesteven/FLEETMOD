using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200001D RID: 29
	[HarmonyPatch(typeof(PLUIPlayMenu), "ActuallyJoinRoom")]
	internal class ActuallyJoinRoom
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00005A60 File Offset: 0x00003C60
		public static bool Prefix(RoomInfo room)
		{
			bool flag = (int)room.CustomProperties["CurrentPlayersPlusBots"] < (int)room.MaxPlayers;
			bool result;
			if (flag)
			{
				bool flag2 = room.CustomProperties.ContainsKey("SteamServerID");
				if (flag2)
				{
					bool flag3 = !SteamManager.Initialized;
					if (flag3)
					{
						PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Failed to join crew! Can't join Secured game when not logged in to Steam!"));
						return false;
					}
					uint num = (uint)((long)room.CustomProperties["SteamServerID"]);
					PLNetworkManager.Instance.ClearSteamAuthSession(num);
					PLNetworkManager.Instance.SetSteamAuthTicket(num);
				}
				bool flag4 = (string)room.CustomProperties["Ship_Type"] == Plugin.myversion;
				if (flag4)
				{
					MyVariables.isrunningmod = true;
					PLNetworkManager.Instance.JoinRoom(room);
					PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
					PLLoader.Instance.IsWaitingOnNetwork = true;
				}
				bool flag5 = !room.CustomProperties["Ship_Type"].ToString().Contains("FLEETMOD");
				if (flag5)
				{
					MyVariables.isrunningmod = false;
					PLNetworkManager.Instance.JoinRoom(room);
					PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
					PLLoader.Instance.IsWaitingOnNetwork = true;
				}
				bool flag6 = (string)room.CustomProperties["Ship_Type"] != Plugin.myversion && room.CustomProperties["Ship_Type"].ToString().Contains(".");
				if (flag6)
				{
					MyVariables.isrunningmod = false;
					PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Sorry, This Server Is Running FleetMod Version " + (string)room.CustomProperties["Ship_Type"] + "\n You Have Version " + Plugin.myversion));
				}
				result = false;
			}
			else
			{
				PLTabMenu.Instance.TimedErrorMsg = "Couldn't join crew! It is full!";
				result = false;
			}
			return result;
		}
	}
}
