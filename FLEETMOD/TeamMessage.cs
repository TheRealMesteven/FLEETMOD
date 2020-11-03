using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200002D RID: 45
	[HarmonyPatch(typeof(PLServer), "TeamMessage")]
	internal class TeamMessage
	{
		// Token: 0x06000058 RID: 88 RVA: 0x00008618 File Offset: 0x00006818
		public static bool Prefix(PLServer __instance, int playerID, string message)
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
				bool flag2 = playerID == 0 && playerFromPlayerID != null;
				if (flag2)
				{
					string text = message.Replace("[&%~[C", "").Replace(" ]&%~]", "");
					PLNetworkManager.Instance.ConsoleText.Insert(0, string.Concat(new object[]
					{
						"[&%~[C",
						"5",
						"  ",
						playerFromPlayerID.GetPlayerName(false),
						" <",
						"Admiral",
						"> ]&%~]: ",
						text
					}));
					PLInGameUI.Instance.ForceChatTextAsDirty = true;
					result = false;
				}
				else
				{
					bool flag3 = playerFromPlayerID != null && playerFromPlayerID.TeamID == 0 && playerID != 0;
					if (flag3)
					{
						string text2 = message.Replace("[&%~[C", "").Replace(" ]&%~]", "");
						bool flag4 = text2.Length > 0;
						if (flag4)
						{
							bool flag5 = text2.Length > 500;
							if (flag5)
							{
								text2 = text2.Substring(0, 500) + " ...";
							}
							PLNetworkManager.Instance.ConsoleText.Insert(0, string.Concat(new object[]
							{
								"[&%~[C",
								playerFromPlayerID.GetClassID(),
								" ",
								playerFromPlayerID.GetPlayerName(false),
								" <",
								playerFromPlayerID.GetClassName(),
								"> ]&%~]: ",
								text2
							}));
							PLInGameUI.Instance.ForceChatTextAsDirty = true;
						}
					}
					result = false;
				}
			}
			return result;
		}
	}
}
