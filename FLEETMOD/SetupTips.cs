using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000003 RID: 3
	[HarmonyPatch(typeof(PLNetworkManager), "Update")]
	internal class SetupTips
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000020CC File Offset: 0x000002CC
		public static bool Prefix(ref int ___TipNumber, ref List<string> ___TipStrings)
		{
			Debug.unityLogger.logEnabled = false;
			bool flag = ___TipStrings.Count > 20;
			if (flag)
			{
				bool flag2 = PLXMLOptionsIO.Instance.CurrentOptions.HasStringValue("TipNumber");
				if (flag2)
				{
					___TipNumber = PLXMLOptionsIO.Instance.CurrentOptions.GetStringValueAsInt("TipNumber");
				}
				else
				{
					___TipNumber = 0;
					PLXMLOptionsIO.Instance.CurrentOptions.SetStringValue("TipNumber", ___TipNumber.ToString());
				}
				___TipStrings.Clear();
				___TipStrings.Add("<color=yellow>Fleet Mod - Press F1 When You Join To Create A New Ship</color>");
				___TipStrings.Add("<color=yellow>Fleet Mod - When You Have More Than One Ship Use Keys < & > (, & .) To Scroll Through The Ships</color>");
			}
			return true;
		}
	}
}
