using System;
using HarmonyLib;
using UnityEngine.UI;

namespace FLEETMOD
{
	// Token: 0x0200000B RID: 11
	[HarmonyPatch(typeof(PLGlobal), "SafeLabelSetText", new Type[]
	{
		typeof(Text),
		typeof(string)
	})]
	internal class SafeLabelSetText
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000028B0 File Offset: 0x00000AB0
		public static bool Prefix(ref Text go, ref string text)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = go != null && go.text != text && text != null;
				if (flag2)
				{
					bool flag3 = text.Contains("*");
					if (flag3)
					{
						return false;
					}
					go.text = text;
				}
				result = false;
			}
			return result;
		}
	}
}
