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
			bool result;
			if ((int)room.CustomProperties["CurrentPlayersPlusBots"] < (int)room.MaxPlayers)
			{
				if (room.CustomProperties.ContainsKey("SteamServerID"))
				{
					if (!SteamManager.Initialized)
					{
						PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Failed to join crew! Can't join Secured game when not logged in to Steam!"));
						return false;
					}
					PLNetworkManager.Instance.ClearSteamAuthSession((uint)((long)room.CustomProperties["SteamServerID"]));
					PLNetworkManager.Instance.SetSteamAuthTicket((uint)((long)room.CustomProperties["SteamServerID"]));
				}
				if ((string)room.CustomProperties["Ship_Type"] == Plugin.myversion)
				{
					MyVariables.isrunningmod = true;
					PLNetworkManager.Instance.JoinRoom(room);
					PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
					PLLoader.Instance.IsWaitingOnNetwork = true;
				}
				if (!room.CustomProperties["Ship_Type"].ToString().Contains("FLEETMOD"))
				{
					MyVariables.isrunningmod = false;
					PLNetworkManager.Instance.JoinRoom(room);
					PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
					PLLoader.Instance.IsWaitingOnNetwork = true;
				}
				if ((string)room.CustomProperties["Ship_Type"] != Plugin.myversion && room.CustomProperties["Ship_Type"].ToString().Contains("."))
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
