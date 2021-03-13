using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "TeamMessage")]
	internal class TeamMessage
	{
		public static bool Prefix(PLServer __instance, int playerID, string message)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(playerID);
				if (playerID == 0 && playerFromPlayerID != null)
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
					return false;
				}
				else
				{
					if (playerFromPlayerID != null && playerFromPlayerID.TeamID == 0 && playerID != 0)
					{
						string text2 = message.Replace("[&%~[C", "").Replace(" ]&%~]", "");
						if (text2.Length > 0)
						{
							if (text2.Length > 500)
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
					return false;
				}
			}
		}
	}
}
