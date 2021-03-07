using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "SetPlayerAsClassID")]
	internal class SetPlayerAsClassID
	{
		public static bool Prefix(int playerID, int classID)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(playerID);
				if (playerFromPlayerID != null)
				{
					bool flag3 = false;
					foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
					{
						bool flag4 = plplayer != null && plplayer.TeamID == 0 && plplayer.GetClassID() == classID && plplayer != playerFromPlayerID && playerFromPlayerID.GetPhotonPlayer().GetScore() == 0;
						if (flag4)
						{
							flag3 = true;
						}
					}
					bool flag5 = !flag3;
					if (flag5)
					{
						playerFromPlayerID.SetClassID(classID);
					}
				}
				result = false;
			}
			return result;
		}
	}
}
