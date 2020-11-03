using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(PLRepairDepot), "Update")]
	internal class UpdatePLRepairDepot
	{
		// Token: 0x06000007 RID: 7 RVA: 0x00002284 File Offset: 0x00000484
		public static bool Prefix(PLRepairDepot __instance, ref PLSensorObjectString[] ___SensorStrings)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				__instance.TargetShip = null;
				float num = 50f;
				bool flag2 = PLEncounterManager.Instance.GetCPEI() != null && __instance.MySensorObject != null;
				if (flag2)
				{
					bool flag3 = !PLEncounterManager.Instance.GetCPEI().MySensorObjects.Contains(__instance.MySensorObject);
					if (flag3)
					{
						PLEncounterManager.Instance.GetCPEI().MySensorObjects.Add(__instance.MySensorObject);
					}
					__instance.MySensorObject.ManualName = "Repair Depot";
					__instance.MySensorObject.ManualEMSignature = 1f;
					__instance.MySensorObject.ManualSensorStrings = ___SensorStrings;
				}
				foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					bool flag4 = plshipInfoBase != null && (plshipInfoBase.Exterior.transform.position - __instance.transform.position).sqrMagnitude < num * num;
					if (flag4)
					{
						__instance.TargetShip = plshipInfoBase;
					}
				}
				result = false;
			}
			return result;
		}
	}
}
