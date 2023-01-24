using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLNetworkManager), "Update")]
	internal class SetupTips
	{
		public static bool Prefix(ref int ___TipNumber, ref List<string> ___TipStrings)
		{
			Debug.unityLogger.logEnabled = false;
			if (___TipStrings.Count > 20)
			{
				if (PLXMLOptionsIO.Instance.CurrentOptions.HasStringValue("TipNumber"))
				{
					___TipNumber = PLXMLOptionsIO.Instance.CurrentOptions.GetStringValueAsInt("TipNumber");
				}
				else
				{
					___TipNumber = 0;
					PLXMLOptionsIO.Instance.CurrentOptions.SetStringValue("TipNumber", ___TipNumber.ToString());
				}
				___TipStrings.Clear();
				___TipStrings.Add("<color=yellow>Fleet Mod - Ask The Admiral When You Join To Create A New Ship</color>");
				___TipStrings.Add("<color=yellow>Fleet Mod - When You Have More Than One Ship Use Keys < & > (, & .) To Scroll Through The Ships</color>");
			}
			return true;
		}
	}
}
