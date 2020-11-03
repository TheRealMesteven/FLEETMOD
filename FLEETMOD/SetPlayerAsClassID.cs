using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000033 RID: 51
	[HarmonyPatch(typeof(PLServer), "SetPlayerAsClassID")]
	internal class SetPlayerAsClassID
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00008F90 File Offset: 0x00007190
		public static bool Prefix(int playerID, int classID)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(playerID);
				bool flag2 = playerFromPlayerID != null;
				if (flag2)
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
