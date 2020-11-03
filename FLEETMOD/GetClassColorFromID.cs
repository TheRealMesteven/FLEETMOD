using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000022 RID: 34
	[HarmonyPatch(typeof(PLPlayer), "GetClassColorFromID")]
	internal class GetClassColorFromID
	{
		// Token: 0x06000042 RID: 66 RVA: 0x000064C8 File Offset: 0x000046C8
		public static bool Prefix(int inID, ref Color __result)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				switch (inID)
				{
				case 0:
					__result = new Color(0f, 0.4f, 1f, 1f);
					break;
				case 1:
					__result = new Color(1f, 1f, 1f, 1f);
					break;
				case 2:
					__result = new Color(0f, 1f, 0f, 1f);
					break;
				case 3:
					__result = new Color(1f, 0f, 0f, 1f);
					break;
				case 4:
					__result = new Color(1f, 0.4f, 0f, 1f);
					break;
				case 5:
					__result = new Color(1f, 1f, 0f, 1f);
					break;
				default:
					__result = new Color(0.4f, 0.4f, 0.4f, 1f);
					break;
				}
				result = false;
			}
			return result;
		}
	}
}
