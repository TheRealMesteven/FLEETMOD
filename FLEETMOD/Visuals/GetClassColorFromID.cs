using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Visuals
{
	[HarmonyPatch(typeof(PLPlayer), "GetClassColorFromID")]
	internal class GetClassColorFromID
	{
		public static bool Prefix(int inID, ref Color __result)
		{
			if (!MyVariables.isrunningmod) return true;
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
				return false;
			}
		}
	}
}
